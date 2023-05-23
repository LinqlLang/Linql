using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    /// <summary>
    /// Represents a property.  For example, in SomeObject.SomeProperty.SomeOtherProperty, SomeProperty and SomeOtherProperty are properties.  
    /// </summary>
    public class LinqlProperty : LinqlExpression
    {
        /// <summary>
        /// The name of the Property
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// This constructor is required for Json serialization/deserialization.  Should probably not use this.
        /// </summary>
        public LinqlProperty() { }

        /// <summary>
        /// Creates a new LinqlProperty.
        /// </summary>
        /// <param name="PropertyName">The name of the Property</param>
        public LinqlProperty(string PropertyName) 
        {
            this.PropertyName = PropertyName;
        }

        public override string ToString()
        {
            return this.PropertyName;
        }

        public override bool Equals(object obj)
        {
            if (obj is LinqlProperty prop)
            {
                return
                    prop.PropertyName == this.PropertyName
                    && base.Equals(prop);
            }
            return false;
        }

        public override bool IsMatch(LinqlExpression ExprssionToCompare, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            if (ExprssionToCompare is LinqlProperty prop)
            {
                bool match = this.PropertyName == prop.PropertyName 
                    && base.IsMatch(prop, FindOption);
                return match;
            }

            return false;
        }
    }
}
