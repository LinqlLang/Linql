using Linql.Client.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Linql.Client
{
    public static class LinqlExtensions
    {
        public static string ToJson(this IQueryable source)
        {
            if (source.Provider is LinqlProvider linqlProvider)
            {
                Linql.Client.Json.LinqlSearch search = linqlProvider.BuildLinqlRequest(source.Expression);    
                string result = JsonSerializer.Serialize(search, linqlProvider.JsonOptions);

                return result;
            }
            else
            {
                return "";
            }
        }

        public static async Task<string> ToJsonAsync(this IQueryable source)
        {
            if (source.Provider is LinqlProvider linqlProvider)
            {
                Linql.Client.Json.LinqlSearch search = linqlProvider.BuildLinqlRequest(source.Expression);
                using (var stream = new MemoryStream())
                {
                    await JsonSerializer.SerializeAsync(stream, search, typeof(Linql.Client.Json.LinqlSearch), linqlProvider.JsonOptions);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            else
            {
                return "";
            }
        }
    }
}
