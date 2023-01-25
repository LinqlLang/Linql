using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linql.Client.Internal;

namespace Linql.Client.Internal
{
    public class UnsupportedIQueryableException : Exception
    {
        public UnsupportedIQueryableException() : base($"This IQueryable does not have an {nameof(IQueryProvider)} that implements {nameof(LinqlProvider)}") { }
    }
}
