using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Client.Exception
{
    /// <summary>
    /// Exception for when an IQueryable isn't provided by a LinqlContext, but a Linql operation is attempted.  
    /// </summary>
    public class UnsupportedExtensionProvider : System.Exception
    {
        public UnsupportedExtensionProvider(IQueryProvider Provider): base($"This exension method can only be used with {nameof(ALinqlContext)} providers.  Found {Provider.GetType().Name}")
        {

        }
    }
}
