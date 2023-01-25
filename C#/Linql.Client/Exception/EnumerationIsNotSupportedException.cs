using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linql.Client.Internal;


namespace Linql.Client.Internal
{
    public class EnumerationIsNotSupportedException : Exception
    {
        public EnumerationIsNotSupportedException() : base($"This IQueryable does not support Enumeration.  Pull the Data into memory before enumeration.") { }
    }
}
