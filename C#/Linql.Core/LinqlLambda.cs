using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Core
{
    /// <summary>
    /// Represents a Lambda Function
    /// </summary>
    public class LinqlLambda : LinqlExpression
    {
        /// <summary>
        /// The parameters of the Lambda Function.  In example: (r, t) => true, the parameters value is [r, t];
        /// </summary>
        public List<LinqlExpression> Parameters { get; set; }

        /// <summary>
        /// The body of the Lambda Function.  In example: (r, t) => true, the body is the constant "true"
        /// </summary>
        public LinqlExpression Body { get; set; }

        public override string ToString()
        {
            return this.Body.ToString();
        }


    }
}
