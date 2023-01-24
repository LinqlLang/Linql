using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Client.Json
{
    public class LinqlConstant : LinqlExpression
    {
        public string ConstantType { get; set; }

        public object Value { get; set; }

        public LinqlConstant(string ConstantType, object Value) 
        {
            this.ConstantType = ConstantType;
            this.Value = Value;
        }
    }
}
