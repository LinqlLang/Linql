using Linql.Client.Internal;
using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Linql.Client
{
    public static class LinqlExtensions
    {
        private static MethodInfo ToListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));


        public static string ToJson(this IQueryable source)
        {
            if (source.Provider is LinqlProvider linqlProvider)
            {
                Linql.Core.LinqlSearch search = source.ToLinqlSearch();
                string result = JsonSerializer.Serialize(search, linqlProvider.JsonOptions);

                return result;
            }
            else
            {
                throw new UnsupportedIQueryableException();
            }
        }

        public static async Task<string> ToJsonAsync(this IQueryable source)
        {
            if (source.Provider is LinqlProvider linqlProvider)
            {
                Linql.Core.LinqlSearch search = source.ToLinqlSearch();
                using (var stream = new MemoryStream())
                {
                    await JsonSerializer.SerializeAsync(stream, search, typeof(Linql.Core.LinqlSearch), linqlProvider.JsonOptions);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            else
            {
                throw new UnsupportedIQueryableException();
            }
        }

        public static LinqlSearch ToLinqlSearch(this IQueryable source)
        {
            if (source.Provider is LinqlProvider linqlProvider)
            {
                Linql.Core.LinqlSearch search = linqlProvider.BuildLinqlRequest(source.Expression, source.GetType().GetEnumerableType());
               
                return search;
            }
            else
            {
                throw new UnsupportedIQueryableException();
            }
        }

        public static IQueryable<TSource> AttachGenericFunction<TSource>(this IQueryable<TSource> source, MethodInfo method)
        {
         
            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    null,
                    method,
                    source.Expression
                    ));
        }

        public static IQueryable<T> ToLinqlList<T>(this IQueryable<T> source)
        {
          
            return source.AttachGenericFunction(LinqlExtensions.ToListMethod.MakeGenericMethod(typeof(T)));
        }

        public static List<TSource> ToList<TSource>(this IEnumerable source)
      => source.OfType<TSource>().ToList();

    }
}
