using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    public class LinqlFunction : LinqlExpression
    {
        public string FunctionName { get; set; }

        public List<LinqlExpression> Arguments { get; set; }

        public LinqlFunction() { }

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
    }
}
