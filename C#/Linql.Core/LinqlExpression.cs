using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Linql.Core
{

    /// <summary>
    /// Base Type of Linql.  All LinqlExpressions derive from this type.
    /// </summary>
    [JsonDerivedType(typeof(LinqlConstant), typeDiscriminator: nameof(LinqlConstant))]
    [JsonDerivedType(typeof(LinqlFunction), typeDiscriminator: nameof(LinqlFunction))]
    [JsonDerivedType(typeof(LinqlLambda), typeDiscriminator: nameof(LinqlLambda))]
    [JsonDerivedType(typeof(LinqlParameter), typeDiscriminator: nameof(LinqlParameter))]
    [JsonDerivedType(typeof(LinqlProperty), typeDiscriminator: nameof(LinqlProperty))]
    [JsonDerivedType(typeof(LinqlBinary), typeDiscriminator: nameof(LinqlBinary))]
    [JsonDerivedType(typeof(LinqlUnary), typeDiscriminator: nameof(LinqlUnary))]
    [JsonDerivedType(typeof(LinqlObject), typeDiscriminator: nameof(LinqlObject))]
    public abstract class LinqlExpression
    {
        /// <summary>
        /// Represents the next linql expression.  
        /// </summary>
        public LinqlExpression Next { get; set; }

        public LinqlExpression() { }

        /// <summary>
        /// Walks the linql expression Next property and returns the last item in that expression, including itself if Next is null.
        /// </summary>
        /// <returns></returns>
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
