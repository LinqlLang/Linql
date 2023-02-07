using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    public class LinqlObject : LinqlExpression
    {
        public LinqlType Type { get; set; }

        public object Value { get; set; }

        public LinqlObject() { }

        public LinqlObject(LinqlType Type, object Value) 
        {
            this.Type = Type;
            this.Value = Value;
        }

        public LinqlObject(Type Type, object Value) : this(new LinqlType(Type), Value)
        {

        }

        public override string ToString()
        {
            return $"LinqlObject {this.Type.ToString()}";
        }
    }

}
