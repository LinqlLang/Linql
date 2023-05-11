using System;
using System.Collections.Generic;
using System.Linq;
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

        public override bool Equals(object obj)
        {
            if (obj is LinqlLambda lam)
            {
                return
                    lam.Body.Equals(this.Body)
                    && lam.Parameters.Count == this.Parameters.Count
                    && lam.Parameters.Zip(this.Parameters, (left, right) => left.Equals(right)).All(r => r)
                    && base.Equals(lam);
            }
            return false;

        }


    }
}
