using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Client
{
    public class UnsupportedIQueryableException : Exception
    {
        public UnsupportedIQueryableException() : base($"This IQueryable does not have an {nameof(IQueryProvider)} that implements {nameof(ALinqlContext)}") { }
    }
}
