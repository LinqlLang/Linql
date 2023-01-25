using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Client.Json
{
    public class LinqlProperty : LinqlExpression
    {
        public string PropertyName { get; set; }

        public LinqlProperty(string PropertyName) 
        {
            this.PropertyName = PropertyName;
        }
    }
}
