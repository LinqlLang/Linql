using Linql.Client.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Linql.Client.Internal
{
    public class LinqlParser : ExpressionVisitor
    {
        internal Stack<LinqlExpression> LinqlStack { get; set; } = new Stack<LinqlExpression>();

        internal Stack<Expression> ExpressionStack { get; set; } = new Stack<Expression>();

        public LinqlExpression Root { get; protected set; }

        protected void PushToStack(LinqlExpression LinqlExpression, Expression CSharpExpression)
        {
            this.LinqlStack.Push(LinqlExpression);
            this.ExpressionStack.Push(CSharpExpression);
        }

        protected void PopStack()
        {
            this.LinqlStack.Pop();
            this.ExpressionStack.Pop();
        }

        protected void AttachToExpression(LinqlExpression ExpressionToAttach)
        {
            LinqlExpression previousExpression = this.LinqlStack.FirstOrDefault();
            if (previousExpression is LinqlLambda lambda)
            {
                lambda.Body = ExpressionToAttach;
            }
            else if (previousExpression is LinqlFunction function)
            {
                if (function.Arguments == null)
                {
                    function.Arguments = new List<LinqlExpression>();
                }
                function.Arguments.Add(ExpressionToAttach);
            }
            else if (previousExpression is LinqlBinary binary)
            {
                if (binary.Left == null)
                {
                    binary.Left = ExpressionToAttach;
                }
                else
                {
                    binary.Right = ExpressionToAttach;
                }
            }
            else if (previousExpression != null)
            {
                previousExpression.Next = ExpressionToAttach;
            }
            else
            {
                this.Root = ExpressionToAttach;
            }

        }

        public LinqlParser(Expression LinqlExpression) : base()
        {
            this.Visit(LinqlExpression);
        }


        public override Expression Visit(Expression expression)
        {
            return base.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {

            if (!(c.Value is Linql.Client.Json.LinqlSearch))
            {
                Expression previous = this.ExpressionStack.FirstOrDefault();

                object value = c.Value;
                string Type = c.Type.Name;

                //if (c.Type.IsClass && previous is MemberExpression exp)
                //{
                //    FieldInfo field = exp.Member.DeclaringType.GetField(exp.Member.Name);
                //    value = field.GetValue(c.Value);
                //    Type = value.GetType().Name;
                    
                //}

                LinqlConstant constant = new LinqlConstant(Type, value);
                this.AttachToExpression(constant);
                this.PushToStack(constant, c);
            }

            return base.VisitConstant(c);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            LinqlFunction function = new LinqlFunction(m.Method.Name);
            this.AttachToExpression(function);
            this.PushToStack(function, m);
            Expression expression = base.VisitMethodCall(m);

            return m;
        }


        //protected override Expression VisitLambda<T>(LambdaExpression lambda)
        //{
        //    return base.VisitLambda<T>(lambda);
        //}

        protected override Expression VisitBinary(BinaryExpression binary)
        {
            string binaryName = Enum.GetName(typeof(ExpressionType), binary.NodeType);
            LinqlBinary linqlBinary = new LinqlBinary(binaryName);
            
            this.AttachToExpression(linqlBinary);
            this.PushToStack(linqlBinary, binary);

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
            
            this.AttachToExpression(linqlLambda);
            this.PushToStack(linqlLambda, lambda);


            base.Visit(lambda.Body);

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
            this.AttachToExpression(param);
            this.PushToStack(param, parameter);
            Expression expression = base.VisitParameter(parameter);

            return parameter;
        }

      
        protected override Expression VisitMember(MemberExpression m)
        {

            LinqlProperty property = new LinqlProperty(m.Member.Name);

            if(m.Member.MemberType == System.Reflection.MemberTypes.Field)
            {
                ExpressionStack.Push(m);
            }

            base.VisitMember(m);
            LinqlExpression previous = this.LinqlStack.First();

            if (previous is LinqlConstant constant)
            {
                object value = constant.Value;

                FieldInfo field = m.Member.DeclaringType.GetField(m.Member.Name);

                if(field != null)
                {
                    value = field.GetValue(value);
                }
                else
                {
                    PropertyInfo propertyInfo = m.Member.DeclaringType.GetProperty(m.Member.Name);
                    value = propertyInfo.GetValue(value);
                }
                
                string Type = value.GetType().Name;

                LinqlConstant linqlConstant = new LinqlConstant(Type, value);
                this.PopStack();
                this.AttachToExpression(linqlConstant);
                this.PushToStack(linqlConstant, m);
            }
            else
            {
                this.AttachToExpression(property);
                this.PushToStack(property, m);
            }
        

            return m;
        }
    }
}
