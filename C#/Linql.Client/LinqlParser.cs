using Linql.Client;
using Linql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Linql.Client
{
    /// <summary>
    /// The Default LinqlParser. LinqlParser extends ExpressionVisitor in order to parse Expression Trees.
    /// </summary>
    public class LinqlParser : ExpressionVisitor
    {
        /// <summary>
        /// A stack of LinqlExpression that were processed.  This is required in order to correctly order the LinqlSearch, as we don't match CSharps order of expressions, but rather, a more human readable format.
        /// </summary>
        internal Stack<LinqlExpression> LinqlStack { get; } = new Stack<LinqlExpression>();

        /// <summary>
        /// A stack of Expressions that were processed.  This is required in order to correctly order the LinqlSearch, as we don't match CSharps order of expressions, but rather, a more human readable format.
        /// </summary>
        internal Stack<Expression> ExpressionStack { get; } = new Stack<Expression>();

        /// <summary>
        /// The root LinqlExpression
        /// </summary>
        public LinqlExpression Root { get; protected set; }

        /// <summary>
        /// The Type of the Root Expression
        /// </summary>
        public Type LinqlSearchRootType { get; set; }

        /// <summary>
        /// Adds a LinqlExpression and an Expression to their respective stacks.
        /// </summary>
        /// <param name="LinqlExpression">LinqlExpression to push</param>
        /// <param name="CSharpExpression">CSharp Expression to push</param>
        protected void PushToStack(LinqlExpression LinqlExpression, Expression CSharpExpression)
        {
            LinqlStack.Push(LinqlExpression);
            ExpressionStack.Push(CSharpExpression);
        }

        /// <summary>
        /// Removes items from both the LinqlStack and the ExpressionStack
        /// </summary>
        protected void PopStack()
        {
            LinqlStack.Pop();
            ExpressionStack.Pop();
        }

        /// <summary>
        /// Attaches an expression to the parser's context.  If Root is null, sets the Root to this expression
        /// </summary>
        /// <param name="ExpressionToAttach">The LinqlExpression to attach to the parser context.</param>
        protected void AttachToExpression(LinqlExpression ExpressionToAttach)
        {
            LinqlExpression previousExpression = LinqlStack.FirstOrDefault();
            if (previousExpression != null)
            {
                previousExpression.Next = ExpressionToAttach;
            }
            else
            {
                Root = ExpressionToAttach;
            }

        }

        /// <summary>
        /// Pops Linql Expression Stack.  
        /// </summary>
        /// <param name="ExpressionToRemove">The Expression to pop</param>
        protected void RemoveFromPrevious(LinqlExpression ExpressionToRemove)
        {
            if (ExpressionToRemove != null)
            {
                PopStack();
            }
        }

        /// <summary>
        /// Creates a new LinqlParser and begins parsing the LinqlExpression
        /// </summary>
        /// <param name="LinqlExpression">The LinqlExpression to parse</param>
        public LinqlParser(Expression LinqlExpression) : base()
        {
            Visit(LinqlExpression);
        }


        public override Expression Visit(Expression expression)
        {
            return base.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            Expression previous = ExpressionStack.FirstOrDefault();

            object value = c.Value;
            LinqlType Type = new LinqlType(c.Type);

            LinqlConstant constant = new LinqlConstant(Type, value);
            if(value is LinqlSearch)
            {
                constant.Value = null;
            }

            AttachToExpression(constant);
            PushToStack(constant, c);

            if(c.Type.GetGenericTypeDefinitionSafe() == typeof(LinqlSearch<>))
            {
                this.LinqlSearchRootType = c.Type.GetGenericArguments().FirstOrDefault();
            }

            return base.VisitConstant(c);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            LinqlFunction function = new LinqlFunction(m.Method.Name);
            LinqlExpression functionCallee;

            function.Arguments = m.Arguments.Select(r =>
            {
                LinqlParser argParser = new LinqlParser(r);

                if (argParser.LinqlSearchRootType != null)
                {
                    this.LinqlSearchRootType = argParser.LinqlSearchRootType;
                }

                return argParser.Root;
            }).ToList();

            if (m.Object != null)
            {
                LinqlParser objectParser = new LinqlParser(m.Object);
                LinqlExpression parsedObject = objectParser.Root;
                functionCallee = parsedObject;
            }
            else
            {
                functionCallee = function.Arguments.FirstOrDefault();
                function.Arguments = function.Arguments.Skip(1).ToList();
            }
           
            LinqlExpression attachTo = functionCallee.GetLastExpressionInNextChain();
            attachTo.Next = function;
            Root = functionCallee;

            return m;
        }


        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType != ExpressionType.Quote)
            {
                string unaryName = Enum.GetName(typeof(ExpressionType), node.NodeType);

                LinqlUnary unary = new LinqlUnary(unaryName);
                AttachToExpression(unary);
                PushToStack(unary, node);
            }
            base.VisitUnary(node);

            return node;
        }

        protected override Expression VisitBinary(BinaryExpression binary)
        {
            string binaryName = Enum.GetName(typeof(ExpressionType), binary.NodeType);
            LinqlBinary linqlBinary = new LinqlBinary(binaryName);

            AttachToExpression(linqlBinary);
            PushToStack(linqlBinary, binary);

            Expression left = binary.Left;
            Expression right = binary.Right;

            LinqlParser leftParser = new LinqlParser(left);
            LinqlParser rightParser = new LinqlParser(right);

            linqlBinary.Left = leftParser.Root;
            linqlBinary.Right = rightParser.Root;

            return binary;
        }

        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            LinqlLambda linqlLambda = new LinqlLambda();

            AttachToExpression(linqlLambda);
            PushToStack(linqlLambda, lambda);

            LinqlParser bodyParser = new LinqlParser(lambda.Body);

            linqlLambda.Body = bodyParser.Root;

            foreach (ParameterExpression parameter in lambda.Parameters)
            {
                LinqlParameter param = new LinqlParameter(parameter.Name);

                if (linqlLambda.Parameters == null)
                {
                    linqlLambda.Parameters = new List<LinqlExpression>();
                }
                linqlLambda.Parameters.Add(param);
            }

            return lambda;
        }


        protected override Expression VisitParameter(ParameterExpression parameter)
        {

            LinqlParameter param = new LinqlParameter(parameter.Name);
            AttachToExpression(param);
            PushToStack(param, parameter);
            Expression expression = base.VisitParameter(parameter);

            return parameter;
        }


        protected override Expression VisitMember(MemberExpression m)
        {

            LinqlProperty property = new LinqlProperty(m.Member.Name);

            if (m.Member.MemberType == MemberTypes.Field)
            {
                ExpressionStack.Push(m);
            }

            //LinqlParser parser = new LinqlParser(m.Expression);
            //LinqlExpression root = parser.Root;
            base.VisitMember(m);
            LinqlExpression previous = LinqlStack.FirstOrDefault();

            if (previous == null)
            {
                previous = new LinqlConstant(new LinqlType(m.Type), m.Type);
            }

            if (previous is LinqlConstant constant && constant.Value != null && !constant.ConstantType.IsList())
            {
                LinqlExpression expression;

                object value = constant.Value;

                if (value != null)
                {

                    FieldInfo field = m.Member.DeclaringType.GetField(m.Member.Name);

                    if (field != null)
                    {
                        value = field.GetValue(value);
                    }
                    else
                    {
                        PropertyInfo propertyInfo = m.Member.DeclaringType.GetProperty(m.Member.Name);
                        value = propertyInfo.GetValue(value);
                    }
                }

                if (value != null)
                {
                    LinqlType Type = new LinqlType(value.GetType());

                    if (value is LinqlObject obj)
                    {
                        expression = new LinqlObject(obj.Type, obj.Value);

                    }
                    else
                    {
                        expression = new LinqlConstant(Type, value);
                    }
                }
                else
                {
                    expression = new LinqlConstant(new LinqlType(typeof(object)), null);
                }
                LinqlExpression previousExpression = LinqlStack.FirstOrDefault();
                RemoveFromPrevious(previousExpression);
                AttachToExpression(expression);
                PushToStack(expression, m);


            }
            else if (previous is LinqlObject && m.Member.DeclaringType.GetGenericTypeDefinitionSafe() == typeof(LinqlObject<>))
            {
                //Ignore TypedValue
            }
            else
            {
                AttachToExpression(property);
                PushToStack(property, m);
            }


            return m;
        }
    }
}
