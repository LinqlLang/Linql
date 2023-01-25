using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Client.Json
{
    public class LinqlUnary : LinqlExpression
    {
        public string UnaryName { get; set; }

        public List<LinqlExpression> Arguments { get; set; }

        public LinqlUnary(string UnaryName, List<LinqlExpression> Arguments = null) 
        {
            this.UnaryName = UnaryName;
            this.Arguments = Arguments;
        }
    }
}
