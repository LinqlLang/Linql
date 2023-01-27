using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
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
        protected Expression Visit(LinqlExpression Expression, Type InputType)
        {
            if(Expression is LinqlLambda lambda)
            {
                //return this.VisitLambda(lambda, InputType);
            }
            else if(Expression is LinqlConstant constant)
            {
                return this.VisitConstant(constant);
            }
            else if (Expression is LinqlParameter param)
            {
                return this.VisitParameter(param, InputType);
            }
            return null;
        }


        protected LambdaExpression VisitLambda(LinqlLambda Lambda, Type InputType, Type FunctionType)
        {
            if(InputType == null)
            {
                throw new Exception("Input type cannot be null when trying to create a Lambda function");
            }

            IEnumerable<ParameterExpression> parameters = Lambda.Parameters.Select(r => this.Visit(r, InputType)).Cast<ParameterExpression>();
            Expression body = this.Visit(Lambda.Body, InputType);

            Type functionTypeConstructed = typeof(Func<,>).MakeGenericType(InputType, FunctionType);
            
            LambdaExpression lambdaExp = Expression.Lambda(functionTypeConstructed, body, parameters);
            return lambdaExp;
        }

        protected Expression VisitConstant(LinqlConstant Constant)
        {
            Type foundType = this.ValidAssemblies.SelectMany(r => r.ExportedTypes.Where(s => s.Name.Contains(Constant.ConstantType.TypeName))).FirstOrDefault();

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
                }
            }

            if(foundType != null)
            {
                return Expression.Constant(value, foundType);
            }
            else
            {
                throw new Exception($"Unable to determine type for constant with type {Constant.ConstantType.TypeName} and value {Constant.Value}");
            }

            
        }

        protected Expression VisitParameter(LinqlParameter Param, Type InputType)
        {
            return Expression.Parameter(InputType, Param.ParameterName);
        }

    }
}
