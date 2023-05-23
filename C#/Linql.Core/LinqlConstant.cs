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

        public override bool Equals(object obj)
        {
            if(obj is LinqlConstant constant)
            {
                if(this.ConstantType.Equals(constant.ConstantType))
                {
                    bool valueEquals = false;

                    if(this.Value == null)
                    {
                        valueEquals = constant.Value == null;
                    }
                    else
                    {
                        valueEquals = this.Value.Equals(constant.Value);
                    }

                    return valueEquals
                        && base.Equals(constant);
                }

                return
                    constant.Value.Equals(this.Value)
                    && constant.ConstantType.Equals(this.ConstantType)
                    && base.Equals(constant);
            }
            return false;
        }

        public override bool IsMatch(LinqlExpression ExprssionToCompare, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            if (ExprssionToCompare is LinqlConstant constant)
            {
                bool equivalentTypes = this.ConstantType.TypesAreEquivalent(constant.ConstantType);
                bool equivalentValues = false;
                if (equivalentTypes == true)
                {
                    if (this.Value == null)
                    {
                        equivalentValues = constant.Value == null;
                    }
                    else if(FindOption == LinqlFindOption.Exact)  
                    {
                        equivalentValues = this.Value.Equals(constant.Value);
                    }
                    else
                    {
                        equivalentValues = equivalentTypes;
                    }
                }
                bool baseEquals = base.IsMatch(ExprssionToCompare, FindOption);

                return equivalentValues && equivalentTypes && baseEquals;
            }

            return false;
        }
    }
}
