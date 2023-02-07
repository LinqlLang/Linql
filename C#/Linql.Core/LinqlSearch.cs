using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Linql.Core
{

    public class LinqlSearch
    {
        public LinqlType Type { get; set; }

        public List<LinqlExpression> Expressions { get; set; } = null;

        public LinqlSearch(LinqlType Type)
        {
            this.Type = Type;
        }

        public LinqlSearch(Type Type) : this(new LinqlType(Type)) { }

        public LinqlSearch()
        {

        }

        public override string ToString()
        {
            return $"LinqlSearch<{this.Type.ToString()}>";
        }
    }
}
