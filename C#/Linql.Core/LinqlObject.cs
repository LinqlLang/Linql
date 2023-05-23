using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    /// <summary>
    /// Represents a LinqlObject.  LinqlObjects are serialized as is, and do not resolve internally.  
    /// For instance, given the expression SomeObject.SomeProperty, if SomeObject is defined as a LinqlObject, SomeObject would be serialized in whole.  The property traversal will then be resolved on the server.  
    /// This is distinctly different if it were defined as a constant, which will resolve on the client first.
    /// </summary>
    public class LinqlObject : LinqlExpression
    {
        /// <summary>
        /// The type of the LinqlObject
        /// </summary>
        public LinqlType Type { get; set; }

        /// <summary>
        /// The value of the LinqlObject
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// This constructor is required for Json serialization/deserialization.  Should probably not use this.
        /// </summary>
        public LinqlObject() { }

        /// <summary>
        /// Creates a new LinqlObject
        /// </summary>
        /// <param name="Type">The type of the LinqlObject.</param>
        /// <param name="Value">The value of the LInqlObject</param>
        public LinqlObject(LinqlType Type, object Value) 
        {
            this.Type = Type;
            this.Value = Value;
        }


        /// <summary>
        /// Creates a new LinqlObject
        /// </summary>
        /// <param name="Type">The type of the LinqlObject</param>
        /// <param name="Value">The  value of the LinqlObject</param>
        public LinqlObject(Type Type, object Value) : this(new LinqlType(Type), Value)
        {

        }

        public override string ToString()
        {
            return $"LinqlObject {this.Type.ToString()}";
        }

        public override bool Equals(object obj)
        {
            if (obj is LinqlObject linqlObj)
            {
                return
                    linqlObj.Value.Equals(this.Value)
                    && linqlObj.Type.Equals(this.Type)
                    && base.Equals(linqlObj);
            }
            return false;
        }

        public override bool IsMatch(LinqlExpression ExprssionToCompare, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            if (ExprssionToCompare is LinqlObject obj)
            {
                bool match = this.Type.Equals(obj.Type)
                    && base.IsMatch(obj, FindOption);
                return match;
            }

            return false;
        }

    }

}
