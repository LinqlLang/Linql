using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Core
{
    public class LinqlType 
    {
        public string TypeName { get; set; }

        public List<LinqlType> GenericParameters { get; protected set; }

        public LinqlType() { }

        public LinqlType(Type Type)
        {
            this.GenericParameters = Type.IsConstructedGenericType ? Type.GetGenericArguments().Select(r => new LinqlType(r)).ToList() : null;

            if (typeof(LinqlSearch).IsAssignableFrom(Type))
            {
                this.TypeName = nameof(LinqlSearch);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(Type))
            {
                this.TypeName = "List";
            }
            else
            {
                this.TypeName = Type.Name;
            }
        }
    }
}
