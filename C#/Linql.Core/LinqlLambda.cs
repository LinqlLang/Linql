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
            List<string> parameterNames = this.Parameters.Where(r => r is LinqlParameter).Select(r => ((LinqlParameter)r).ParameterName).ToList();
            string paramString = String.Join(",", parameterNames);
            return $"({paramString}) => {this.Body.ToString()}";
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

        public override List<LinqlFindResult> Find(LinqlExpression ExpressionToFind, LinqlFindResult CurrentResult = null)
        {
            List<LinqlFindResult> results = new List<LinqlFindResult>();

            if (ExpressionToFind is LinqlLambda lam)
            {
                bool paramCountsMatch = this.Parameters.Count == lam.Parameters.Count;
                if (paramCountsMatch)
                {
                    results.AddRange(this.FindMatchFound(ExpressionToFind, CurrentResult));
                }
            }

            return results;
        }

        protected override List<LinqlFindResult> FindMatchFound(LinqlExpression ExpressionToFind, LinqlFindResult CurrentResult = null)
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

            if (ExpressionToFind is LinqlLambda lam)
            {

                if (this.Body == null && lam.Body == null)
                {
                    results.Add(CurrentResult);
                }
                else if (this.Body != null)
                {
                    results.AddRange(this.Body.Find(lam.Body, CurrentResult));
                }

            }
            return results;
        }
    }
}
