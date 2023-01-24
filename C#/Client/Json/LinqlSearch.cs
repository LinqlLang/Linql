using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Client.Json
{
    public class LinqlSearch
    {
        public string Type { get; set; }

        public List<LinqlExpression> Expressions { get; set; } = null;

        public LinqlSearch(string Type)
        {
            this.Type = Type;
        }
    }
}
