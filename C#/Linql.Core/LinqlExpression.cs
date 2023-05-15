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

        public virtual List<LinqlFindResult> Find(LinqlExpression ExpressionToFind)
        {
            List<LinqlFindResult> results = this.Find(ExpressionToFind, ExpressionToFind);
            return results;
        }

        public virtual List<LinqlFindResult> Find(LinqlExpression OriginalExpressionToFind, LinqlExpression ExpressionSegmentToFind, LinqlFindResult CurrentResult = null)
        {
            List<LinqlFindResult> results = new List<LinqlFindResult>();
            bool originalMatch = this.IsMatch(OriginalExpressionToFind);
            bool segmentMatch = this.IsMatch(ExpressionSegmentToFind);

            if (originalMatch)
            {
                List<LinqlFindResult> originalResults = this.FindMatchFound(OriginalExpressionToFind, OriginalExpressionToFind);
                results.AddRange(originalResults);
            }
            if(segmentMatch)
            {
                List<LinqlFindResult> segmentResults = this.FindMatchFound(OriginalExpressionToFind, ExpressionSegmentToFind, CurrentResult);
                results.AddRange(segmentResults);
            }

            return results;
        }

        public virtual bool IsMatch(LinqlExpression ExpressionToCompare)
        {
            if(this.Next == null && ExpressionToCompare.Next != null)
            {
                return false;
            }
            else if(ExpressionToCompare.Next == null)
            {
                return true;
            }

            return this.Next.IsMatch(ExpressionToCompare.Next);
        }

        protected virtual LinqlFindResult AddMatchToResult(LinqlExpression ExpressionToAdd, LinqlFindResult CurrentResult = null)
        {
            if (CurrentResult == null)
            {
                CurrentResult = new LinqlFindResult(this);
            }
            else
            {
                CurrentResult.ExpressionPath.Add(this);
            }

            return CurrentResult;
        }

        protected virtual List<LinqlFindResult> ContinueFind(LinqlExpression OriginalExpression, LinqlExpression ExpressionSegmentToFind, LinqlFindResult CurrentResult = null)
        {
            List<LinqlFindResult> results = new List<LinqlFindResult>();

            if (ExpressionSegmentToFind.Next == null)
            {
                CurrentResult.EndOfExpression = this;
                results.Add(CurrentResult);
            }
            else if (ExpressionSegmentToFind.Next != null && this.Next != null)
            {
                results.AddRange(this.Next.Find(OriginalExpression, ExpressionSegmentToFind.Next, CurrentResult));
            }

            return results;
        }
 
        protected virtual List<LinqlFindResult> FindMatchFound(LinqlExpression OriginalExpression, LinqlExpression ExpressionSegmentToFind, LinqlFindResult CurrentResult = null)
        {
            CurrentResult = this.AddMatchToResult(ExpressionSegmentToFind, CurrentResult);
            return this.ContinueFind(OriginalExpression, ExpressionSegmentToFind, CurrentResult);
        }

    }
}
