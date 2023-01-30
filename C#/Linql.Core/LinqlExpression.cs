using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Linql.Core
{
    [JsonDerivedType(typeof(LinqlConstant), typeDiscriminator: nameof(LinqlConstant))]
    [JsonDerivedType(typeof(LinqlFunction), typeDiscriminator: nameof(LinqlFunction))]
    [JsonDerivedType(typeof(LinqlLambda), typeDiscriminator: nameof(LinqlLambda))]
    [JsonDerivedType(typeof(LinqlParameter), typeDiscriminator: nameof(LinqlParameter))]
    [JsonDerivedType(typeof(LinqlProperty), typeDiscriminator: nameof(LinqlProperty))]
    [JsonDerivedType(typeof(LinqlBinary), typeDiscriminator: nameof(LinqlBinary))]
    [JsonDerivedType(typeof(LinqlUnary), typeDiscriminator: nameof(LinqlUnary))]
    [JsonDerivedType(typeof(LinqlObject), typeDiscriminator: nameof(LinqlObject))]

    public class LinqlExpression
    {
        public LinqlExpression Next { get; set; }

        public LinqlExpression() { }

        public LinqlExpression GetLastExpressionInNextChain()
        {
            if(this.Next == null)
            {
                return this;
            }
            return this.Next.GetLastExpressionInNextChain();
        }
    }
}
