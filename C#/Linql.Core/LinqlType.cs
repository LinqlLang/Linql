using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    public class LinqlType 
    {
        readonly static string ListType = "List";

        public string TypeName { get; set; }

        public List<LinqlType> GenericParameters { get; set; }

        public LinqlType() { }

        public LinqlType(Type Type)
        {
            this.GenericParameters = Type.IsConstructedGenericType ? Type.GetGenericArguments().Select(r => new LinqlType(r)).ToList() : null;

            if (typeof(LinqlSearch).IsAssignableFrom(Type))
            {
                this.TypeName = nameof(LinqlSearch);
            }
            else if (typeof(string).IsAssignableFrom(Type))
            {
                this.TypeName = Type.Name;
            }
            else if (Type.IsDictionary())
            {
                this.TypeName = "Dictionary";
            }
            else if (typeof(IEnumerable).IsAssignableFrom(Type))
            {
                this.TypeName = LinqlType.ListType;
            }
            else
            {
                this.TypeName = Type.Name;
            }
        }

        public bool IsList()
        {
            return this.TypeName == LinqlType.ListType;
        }

        public override string ToString()
        {
            IEnumerable<string> genericTypes = this.GenericParameters?.Select(r => r.ToString());
            if (genericTypes != null && genericTypes.Any())
            {
                string genericTypesJoin = String.Join(", ", genericTypes);
                return $"{this.TypeName}<{genericTypesJoin}>";
            }

            return this.TypeName;
        }

        public override bool Equals(object obj)
        {
            if(obj is LinqlType other)
            {
                return other.ToString() == this.ToString();
            }
            return false;
        }
    }
}
