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
    }
}
