using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Core
{
    public class LinqlObject : LinqlExpression
    {
        public LinqlType Type { get; set; }

        public object Value { get; set; }

        public LinqlObject(LinqlType Type, object Value) 
        {
            this.Type = Type;
            this.Value = Value;
        }

        public LinqlObject(Type Type, object Value) : this(new LinqlType(Type), Value)
        {

        }
    }

}
