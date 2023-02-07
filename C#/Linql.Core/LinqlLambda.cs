using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Core
{
    public class LinqlLambda : LinqlExpression
    {
        public List<LinqlExpression> Parameters { get; set; }

        public LinqlExpression Body { get; set; }

        public override string ToString()
        {
            return this.Body.ToString();
        }


    }
}
