using Linql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linql.Server
{
    public static class LinqlCompiler
    {
        public static void Execute(this LinqlSearch Search, IQueryable Queryable)
        {
            Search.Expressions.ForEach(r =>
            {
                if(r is LinqlFunction function)
                {

                }
                else if(r is LinqlConstant constant)
                {

                }
            });
        }
    }
}
