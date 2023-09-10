using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Linql.Server
{
    public partial class LinqlCompiler
    {

        /// <summary>
        /// Visit Root.  In the Visitor Pattern, you can call this method generically and it will dispatch to the correct branch. 
        /// </summary>
        /// <param name="Expression">The LinqlExpression</param>
        /// <param name="InputType">The Root Type of this Expression Tree.</param>
        /// <param name="Previous">The last expression in this chain.</param>
        /// <returns>A C# Expression</returns>
        /// <exception cref="Exception">Unsupported expression type.</exception>
        protected Expression Visit(LinqlExpression Expression, Type InputType, Expression Previous = null)
        {

            if (Expression is LinqlConstant constant)
            {
                return this.VisitConstant(constant, InputType, Previous);
            }
            else if (Expression is LinqlObject obj)
            {
                return this.VisitObject(obj, InputType, Previous);
            }
            else if (Expression is LinqlParameter param)
            {
                return this.VisitParameter(param, InputType, Previous);
            }
            else if (Expression is LinqlProperty property)
            {
                return this.VisitProperty(property, InputType, Previous);
            }
            else if (Expression is LinqlBinary binary)
            {
                return this.VisitBinary(binary, InputType, Previous);
            }
            else if (Expression is LinqlUnary unary)
            {
                return this.VisitUnary(unary, InputType, Previous);
            }
            else if (Expression is LinqlFunction function)
            {
                return this.VisitFunction(function, InputType, Previous);
            }
            else if (Expression is LinqlLambda lam)
            {
                return this.VisitLambda(lam, InputType, Previous);
            }
            else
            {
                throw new Exception($"{this.GetType().Name} does not support expression of type {Expression.GetType().Name}");
            }


        }

        protected LambdaExpression VisitLambda(LinqlLambda Lambda, Type InputType, Expression Previous = null)
        {
            if (InputType == null)
            {
                throw new Exception("Input type cannot be null when trying to create a Lambda function");
            }

            List<ParameterExpression> parameters = Lambda.Parameters.Select(r => this.Visit(r, InputType)).Cast<ParameterExpression>().ToList();

            parameters.ForEach(r =>
            {
                this.Parameters.Add(r.Name, r);
            });

            LinqlCompiler bodyCompiler = new LinqlCompiler(this, new Dictionary<string, ParameterExpression>(this.Parameters));

            Expression body = bodyCompiler.Visit(Lambda.Body, InputType);

            Type functionTypeConstructed = typeof(Func<,>).MakeGenericType(InputType, body.Type);

            LambdaExpression lambdaExp = Expression.Lambda(functionTypeConstructed, body, parameters);
            return lambdaExp;
        }

        protected Expression VisitConstant(LinqlConstant Constant, Type InputType, Expression Previous = null)
        {
            Type foundType = this.GetLinqlType(Constant.ConstantType);

            object value = Constant.Value;

            if (value is JsonElement json)
            {
                value = json.Deserialize(foundType, this.JsonOptions);
            }

            Expression expression = Expression.Constant(value, foundType);

            if(Constant.Next != null && value != null)
            {
                expression = this.Visit(Constant.Next, InputType, expression);
            }
            return expression;
        }

        protected Expression VisitFunction(LinqlFunction Function, Type InputType, Expression Previous)
        {
            List<Expression> argExpressions = new List<Expression>();

            Function.Arguments?.ForEach(r =>
            {
                Expression argExpression;
                if (r is LinqlLambda lambda)
                {
                    Type inputType = InputType;

                    argExpression  = this.VisitLambda(r as LinqlLambda, Previous.Type.GetEnumerableType());
                }
                else
                {
                    argExpression = this.Visit(r, InputType);
                }

                argExpressions.Add(argExpression);
            });

            Expression objectExpression = Previous;

            MethodInfo foundMethod = this.FindMethod(objectExpression.Type, Function, argExpressions);
            Type genericMethodType = objectExpression.Type;


            if (foundMethod.IsStatic)
            {
                argExpressions.Insert(0, objectExpression);
                objectExpression = null;
            }

            if (foundMethod.IsGenericMethod)
            {
               foundMethod = this.CompileGenericMethod(foundMethod, argExpressions.FirstOrDefault().Type.GetEnumerableType(), argExpressions.Skip(1));
            }

            Expression functionExp = Expression.Call(objectExpression, foundMethod, argExpressions);

            if(Function.Next != null)
            {
                functionExp = this.Visit(Function.Next, functionExp.Type, functionExp);
             
            }

            return functionExp;
        }

        protected Expression VisitObject(LinqlObject Obj, Type InputType, Expression Previous = null)
        {
            Type foundType = this.GetLinqlType(Obj.Type);


            JsonElement jsonElement = (JsonElement)Obj.Value;
            object value = jsonElement.Deserialize(foundType, this.JsonOptions);
            Expression expression = Expression.Constant(value, foundType);

            if (Obj.Next != null)
            {
                expression = this.Visit(Obj.Next, InputType, expression);
            }
            return expression;

        }

        protected Expression VisitUnary(LinqlUnary Unary, Type InputType, Expression Previous = null)
        {
            MethodInfo expressionType = typeof(Expression).GetMethod(Unary.UnaryName, new Type[] { typeof(Expression) });

            if (expressionType == null)
            {
                throw new Exception($"Unable to find Unary Method {Unary.UnaryName}");
            }
            if (Unary.Next == null)
            {
                throw new Exception($"Unary {Unary.UnaryName} cannot be the last statement of an expression.  It must have a Next value.");
            }

            Expression expression = this.Visit(Unary.Next, InputType, null);

            expression = (Expression)expressionType.Invoke(null, new object[] { expression });

            return expression;

        }

        private Type GetLinqlType(LinqlType LinqlType)
        {

            Type foundType = this.GetTypeFromLinqlObject(LinqlType);

            if (foundType == null)
            {
                throw new Exception($"Unable to find Type {LinqlType.TypeName} in the LinqlContext");
            }

            if (LinqlType.GenericParameters != null)
            {
                List<Type> genericArguments = LinqlType.GenericParameters.Select(r => this.GetLinqlType(r)).ToList();
                foundType = foundType.MakeGenericType(genericArguments.ToArray());
            }
            return foundType;
        }

        protected Expression VisitParameter(LinqlParameter Param, Type InputType, Expression Previous = null)
        {
            Expression parameter;

            if (this.Parameters.ContainsKey(Param.ParameterName))
            {
                parameter = this.Parameters[Param.ParameterName];
            }
            else
            {
                parameter = Expression.Parameter(InputType, Param.ParameterName);
            }

            if (Param.Next != null)
            {
                parameter = this.Visit(Param.Next, InputType, parameter);
            }

            return parameter;
        }

        protected Expression VisitProperty(LinqlProperty Property, Type InputType, Expression Previous = null)
        {
            if (Previous == null)
            {
                throw new Exception("Attempted to access a property on a null Expression");
            }

            PropertyInfo propertyInfo = Previous.Type.GetProperty(Property.PropertyName);
            Expression property = Expression.Property(Previous, propertyInfo);

            if (Property.Next != null)
            {
                property = this.Visit(Property.Next, InputType, property);
            }

            return property;
        }

        protected Expression VisitBinary(LinqlBinary Binary, Type InputType, Expression Previous = null)
        {

            LinqlCompiler leftC = new LinqlCompiler(this, new Dictionary<string, ParameterExpression>(this.Parameters));
            LinqlCompiler rightC = new LinqlCompiler(this, new Dictionary<string, ParameterExpression>(this.Parameters));

            Expression left = leftC.Visit(Binary.Left, InputType);
            Expression right = rightC.Visit(Binary.Right, InputType);

            List<MethodInfo> foundMethods = typeof(Expression).GetMethods().Where(r => r.Name == Binary.BinaryName).ToList();

            MethodInfo binaryMethod = foundMethods.FirstOrDefault();

            left = this.HandleNullConstants(left, right);
            right = this.HandleNullConstants(right, left);

            left = this.HandleDates(left, right);
            right = this.HandleDates(right, left);

            Expression binaryExpression = (Expression)binaryMethod.Invoke(null, new object[] { left, right });
            return binaryExpression;

        }

        private Expression HandleDates(Expression left, Expression right)
        {
            if(left.Type == typeof(string) && right.Type == typeof(DateTime) && left is ConstantExpression constant)
            {
                DateTime stringToTime = DateTime.Parse(constant.Value as string);
                return Expression.Constant(stringToTime);
            }
            return left;
        }

        private Expression HandleNullConstants(Expression ExpressionToCheck, Expression CorrespondingExpression)
        {
            Expression expressionToCheck = ExpressionToCheck;
            if (expressionToCheck is ConstantExpression constant && constant.Value == null && CorrespondingExpression.Type.IsValueType)
            {
                object nonNullInstance = Activator.CreateInstance(CorrespondingExpression.Type);
                expressionToCheck = Expression.Constant(nonNullInstance);
            }
            return expressionToCheck;
        }

    }
}
