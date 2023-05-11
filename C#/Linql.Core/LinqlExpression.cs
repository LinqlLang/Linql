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

        public override bool Equals(object obj)
        {
            if(obj is LinqlExpression exp)
            {
                if(this.Next == null)
                {
                    return exp.Next == null;
                }
                return this.Next.Equals(exp.Next);
            }
            return false;
        }

        /// <summary>
        /// Searches for an expression chain inside this expression chain
        /// </summary>
        /// <param name="ExpressionToFind">The expression chain to match</param>
        /// <param name="CurrentResult">Indicates that a chain segment as already been found</param>
        /// <returns>A list of LinqlFindResults.  This can be a list if the expression branches, and so that we can support finding multiple instances of a chain</returns>
        public virtual List<LinqlFindResult> Find(LinqlExpression ExpressionToFind, LinqlFindResult CurrentResult = null)
        {
            List<LinqlFindResult> results = new List<LinqlFindResult>();

            return results;
        }

        protected virtual List<LinqlFindResult> FindMatchFound(LinqlExpression ExpressionToFind, LinqlFindResult CurrentResult = null)
        {
            List<LinqlFindResult> results = new List<LinqlFindResult>();

            if (CurrentResult == null)
            {
                CurrentResult = new LinqlFindResult(this);
            }
            else
            {
                CurrentResult.ExpressionPath.Add(this);
            }

            if(ExpressionToFind.Next == null)
            {
                CurrentResult.EndOfExpression = this;
                results.Add(CurrentResult);
            }
            else if(ExpressionToFind.Next != null && this.Next != null)
            {
                results.AddRange(this.Next.Find(ExpressionToFind.Next, CurrentResult));
            }

            return results;
        }

    }
}
