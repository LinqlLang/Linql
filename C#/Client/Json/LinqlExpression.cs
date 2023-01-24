using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Client.Json
{
    public abstract class LinqlExpression
    {
        public LinqlExpression Next { get; set; }
    }
}
