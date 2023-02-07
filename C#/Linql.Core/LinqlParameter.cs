using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    public class LinqlParameter : LinqlExpression
    {
        public string ParameterName { get; set; }

        public LinqlParameter() { }

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
