using Linql.Client.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linql.Client
{
    public static class LinqlExtensions
    {
        public static string ToJson(this IQueryable source)
        {
            if (source.Provider is LinqlProvider linqlProvider)
            {
                Linql.Client.Json.LinqlSearch search = linqlProvider.BuildLinqlRequest(source.Expression);    
                return JsonSerializer.Serialize(search, linqlProvider.JsonOptions);
            }
            else
            {
                return "";
            }
        }
    }
}
