using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    /// <summary>
    /// Represents a unary expression. For example, ! (Not) is a unary expression. 
    /// </summary>
    public class LinqlUnary : LinqlExpression
    {
        /// <summary>
        /// The name of the unary
        /// </summary>
        public string UnaryName { get; set; }


        /// <summary>
        /// Arguments to the unary expression
        /// </summary>
        public List<LinqlExpression> Arguments { get; set; }

        /// <summary>
        /// This constructor is required for Json serialization/deserialization.  Should probably not use this.
        /// </summary>
        public LinqlUnary()
        {

        }

        /// <summary>
        /// Creates a new LinqlUnary
        /// </summary>
        /// <param name="UnaryName">The name of the Unary Operation</param>
        /// <param name="Arguments">The arguments to the Unary Operation</param>
        public LinqlUnary(string UnaryName, List<LinqlExpression> Arguments = null)
        {
            this.UnaryName = UnaryName;
            this.Arguments = Arguments;
        }

        public override bool Equals(object obj)
        {
            if (obj is LinqlUnary un)
            {
                return
                    un.UnaryName == this.UnaryName
                    && base.Equals(un);
            }
            return false;
        }

        public override bool IsMatch(LinqlExpression ExprssionToCompare, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            if (ExprssionToCompare is LinqlUnary un)
            {
                bool match = this.UnaryName == un.UnaryName
                    && base.IsMatch(un, FindOption);
                return match;
            }

            return false;
        }

        protected override List<LinqlExpression> ContinueFind(LinqlExpression ExpressionToFind, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            List<LinqlExpression> results = new List<LinqlExpression>();

            List<LinqlExpression> argMatches = this.Arguments.SelectMany(r =>
            {
                return r.Find(ExpressionToFind, FindOption);
            }).ToList();

            results.AddRange(argMatches);


            List<LinqlExpression> baseMatch = base.ContinueFind(ExpressionToFind, FindOption);
            results.AddRange(baseMatch);

            return results;
        }

    }
}
