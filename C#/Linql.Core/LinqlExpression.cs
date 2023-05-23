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
            if (this.Next == null)
            {
                return this;
            }
            return this.Next.GetLastExpressionInNextChain();
        }

        public override bool Equals(object obj)
        {
            if (obj is LinqlExpression exp)
            {
                if (this.Next == null)
                {
                    return exp.Next == null;
                }
                return this.Next.Equals(exp.Next);
            }
            return false;
        }
        public virtual List<LinqlExpression> Find(LinqlExpression ExpressionToFind, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            List<LinqlExpression> results = new List<LinqlExpression>();
            bool originalMatch = this.IsMatch(ExpressionToFind, FindOption);

            if (originalMatch)
            {
                results.Add(this);
            }

            List<LinqlExpression> nestedResults = this.ContinueFind(ExpressionToFind, FindOption);
            results.AddRange(nestedResults);
            return results;
        }

        public virtual bool IsMatch(LinqlExpression ExpressionToCompare, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            if(this.Next == null && ExpressionToCompare.Next != null)
            {
                return false;
            }
            else if(ExpressionToCompare.Next == null)
            {
                return true;
            }

            return this.Next.IsMatch(ExpressionToCompare.Next, FindOption);
        }

        protected virtual List<LinqlExpression> ContinueFind(LinqlExpression ExpressionToFind, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            List<LinqlExpression> results = new List<LinqlExpression>();

            if(this.Next != null)
            {
                List<LinqlExpression> nextResult = this.Next.Find(ExpressionToFind, FindOption);
                results.AddRange(nextResult);
            }

            return results;
        } 
    }
}
