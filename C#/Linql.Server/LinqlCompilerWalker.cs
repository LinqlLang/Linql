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
        protected Expression Visit(LinqlExpression Expression, Type InputType, Expression Previous = null)
        {
            if(Expression is LinqlLambda lambda)
            {
                //return this.VisitLambda(lambda, InputType);
            }
            else if(Expression is LinqlConstant constant)
            {
                return this.VisitConstant(constant, InputType, Previous);
            }
            else if (Expression is LinqlObject obj)
            {
                return this.VisitObject(obj, InputType,Previous);
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
            return null;
        }


        protected LambdaExpression VisitLambda(LinqlLambda Lambda, Type InputType, Type FunctionType, Expression Previous = null)
        {
            if(InputType == null)
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

            Type functionTypeConstructed = typeof(Func<,>).MakeGenericType(InputType, FunctionType);
            
            LambdaExpression lambdaExp = Expression.Lambda(functionTypeConstructed, body, parameters);
            return lambdaExp;
        }

        protected Expression VisitConstant(LinqlConstant Constant, Type InputType, Expression Previous = null)
        {
            Type foundType = this.GetLinqlType(Constant.ConstantType);

            object value = Constant.Value;

            if(value is JsonElement json)
            {
                switch (json.ValueKind) {

                    case JsonValueKind.True:
                        value = true;
                        break;
                    case JsonValueKind.False:
                        value = false;
                        break;
                    case JsonValueKind.Number:
                        value = json.GetInt32();
                        break;
                    default:
                        throw new Exception($"No support for json ValueKind {json.ValueKind}");
                }
            }

            if(foundType != null)
            {
                Expression expression = Expression.Constant(value, foundType);

                if (Constant.Next != null)
                {
                    expression = this.Visit(Constant.Next, InputType, expression);
                }
                return expression;
            }
            else
            {
                throw new Exception($"Unable to determine type for constant with type {Constant.ConstantType.TypeName} and value {Constant.Value}");
            }

            
        }

        protected Expression VisitObject(LinqlObject Obj, Type InputType, Expression Previous = null)
        {
            Type foundType = this.GetLinqlType(Obj.Type);

            if (foundType != null)
            {
                JsonElement jsonElement = (JsonElement) Obj.Value;
                object value = jsonElement.Deserialize(foundType);
                Expression expression = Expression.Constant(value, foundType);

                if (Obj.Next != null)
                {
                    expression = this.Visit(Obj.Next, InputType, expression);
                }
                return expression;
            }
            else
            {
                throw new Exception($"Unable to determine type for constant with type {Obj.Type.TypeName} and value {Obj.Value}");
            }
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
                List<Type> genericArguments = LinqlType.GenericParameters.Select(r => this.GetLinqlType(LinqlType)).ToList();
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
            if(Previous == null)
            {
                throw new Exception("Attempted to access a property on a null Expression");
            }

            PropertyInfo propertyInfo = Previous.Type.GetProperty(Property.PropertyName);
            Expression property = Expression.Property(Previous, propertyInfo);

            if(Property.Next != null)
            {
                property = this.Visit(Property.Next, InputType, property);
            }

            return property;
        }

        protected Expression VisitBinary(LinqlBinary Binary, Type InputType, Expression Previous = null)
        {

            LinqlCompiler leftC = new LinqlCompiler(this, new Dictionary<string, ParameterExpression>(this.Parameters));
            LinqlCompiler rightC = new LinqlCompiler(this, new Dictionary<string, ParameterExpression>(this.Parameters));

            Expression left = leftC.Visit(Binary.Left, InputType, Previous);
            Expression right = leftC.Visit(Binary.Right, InputType, Previous);

            List<MethodInfo> foundMethods = typeof(Expression).GetMethods().Where(r => r.Name == Binary.BinaryName).ToList();

            MethodInfo binaryMethod = foundMethods.FirstOrDefault();

            Expression binaryExpression = (Expression) binaryMethod.Invoke(null, new object[] { left, right });
            return binaryExpression;
          
        }

    }
}
