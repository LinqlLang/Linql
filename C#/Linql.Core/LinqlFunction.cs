using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    /// <summary>
    /// Represents a function.  Lambdas are not Functions, but rather arguments to a function generally.
    /// </summary>
    public class LinqlFunction : LinqlExpression
    {
        /// <summary>
        /// The name of the function
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// Arguments to the function
        /// </summary>
        public List<LinqlExpression> Arguments { get; set; }

        /// <summary>
        /// This constructor is required for Json serialization/deserialization.  Should probably not use this.
        /// </summary>
        public LinqlFunction() { }

        /// <summary>
        /// Creates a LinqlFunction.
        /// </summary>
        /// <param name="FunctionName">The name of the Function</param>
        /// <param name="Arguments">The Arguments to the function</param>
        public LinqlFunction(string FunctionName, List<LinqlExpression> Arguments = null) 
        {
            this.FunctionName = FunctionName;
            this.Arguments = Arguments;
        }

        public override string ToString()
        {
            int argumentCount = 0;

            if(this.Arguments != null)
            {
                argumentCount = this.Arguments.Count;
            }

            return $"{FunctionName}({argumentCount})";
        }

        public override bool Equals(object obj)
        {
            if (obj is LinqlFunction fun)
            {
                return
                    fun.FunctionName == this.FunctionName
                    && fun.Arguments.Count == this.Arguments.Count 
                    && fun.Arguments.Zip(this.Arguments, (left, right) => left.Equals(right)).All( r => r)
                    && base.Equals(fun);
            }
            return false;
        }

        public override bool IsMatch(LinqlExpression ExprssionToCompare)
        {
            if (ExprssionToCompare is LinqlFunction fun)
            {

                bool match = fun.FunctionName == this.FunctionName
                && fun.Arguments.Count == this.Arguments.Count
                && this.Arguments.Zip(fun.Arguments, (left, right) => left.IsMatch(right)).All(r => r);

                return match;
            }

            return false;
        }
    }
}
