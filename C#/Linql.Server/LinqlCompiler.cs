using Linql.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Server
{
    public class LinqlCompiler
    {
        protected LinqlSearch Search { get; set; }

        public LinqlCompiler(LinqlSearch Search)
        {
            this.Search = Search;
        }
    }
}
