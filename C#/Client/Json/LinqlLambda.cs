using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Client.Json
{
    public class LinqlLambda : LinqlExpression
    {
        public List<LinqlExpression> Parameters { get; set; }

        public LinqlExpression Body { get; set; }
    
    }
}
