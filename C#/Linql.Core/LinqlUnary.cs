using System;
using System.Collections.Generic;
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
    }
}
