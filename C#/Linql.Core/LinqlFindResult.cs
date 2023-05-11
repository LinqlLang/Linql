using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Core
{
    public class LinqlFindResult
    {
        public List<LinqlExpression> ExpressionPath { get; set; } = new List<LinqlExpression>();

        public LinqlExpression StartOfExpression { get; set; }

        public LinqlExpression EndOfExpression { get; set; }

        public LinqlFindResult(LinqlExpression StartChain)
        {
            this.ExpressionPath.Add(StartChain);
            this.StartOfExpression = StartChain;
        }

        /// <summary>
        /// Clones a LinqlFindResult. 
        /// </summary>
        /// <returns>Returns a new LinqlFindResult that matches the source</returns>
        public LinqlFindResult Clone()
        {
            LinqlFindResult clone = new LinqlFindResult(this.StartOfExpression);
            clone.ExpressionPath = new List<LinqlExpression>(this.ExpressionPath);
            return clone;
        }
    }
}
