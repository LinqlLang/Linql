using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    /// <summary>
    /// Represents a parameter.  In example, given (r) => true, the parameter is r.
    /// </summary>
    public class LinqlParameter : LinqlExpression
    {
        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParameterName { get; set; }

        public LinqlParameter() { }

        /// <summary>
        /// Creates a new LinqlParameter
        /// </summary>
        /// <param name="ParameterName">The name of the Parameter</param>
        public LinqlParameter(string ParameterName) 
        {
            this.ParameterName = ParameterName;
        }

        public override string ToString()
        {
            return this.ParameterName;
        }
    }
}
