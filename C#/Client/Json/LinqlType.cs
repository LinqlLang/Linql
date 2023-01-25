using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Client.Json
{
    public class LinqlType 
    {
        public string TypeName { get; set; }

        public List<LinqlType> GenericParameters { get; set; }

        public LinqlType(string TypeName, List<LinqlType> GenericParameters) 
        {
            this.TypeName = TypeName;
            this.GenericParameters = GenericParameters;
        }
    }
}
