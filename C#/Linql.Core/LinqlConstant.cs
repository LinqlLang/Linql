using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    /// <summary>
    /// Represents a Constant expression.  
    /// </summary>
    public class LinqlConstant : LinqlExpression
    {
        /// <summary>
        /// The type of the constant. 
        /// </summary>
        public LinqlType ConstantType { get; set; }

        /// <summary>
        /// The value of the constant
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// This constructor is required for Json serialization/deserialization.  Should probably not use this.
        /// </summary>
        public LinqlConstant() { }


        /// <summary>
        /// Creates a new LinqlConstant.
        /// </summary>
        /// <param name="ConstantType">The Type of the Constant.  Types can be converted to a LinqlType simply by calling new LinqlType(someCSharptype)</param>
        /// <param name="Value">The value of the constant</param>
        public LinqlConstant(LinqlType ConstantType, object Value) 
        {
            this.ConstantType = ConstantType;
            this.Value = Value;
        }

        public override string ToString()
        {
            return $"LinqlConstant {this.ConstantType.ToString()}";
        }
    }
}
