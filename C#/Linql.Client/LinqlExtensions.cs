using Linql.Client.Internal;
using Linql.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Linql.Client
{
    public static class LinqlExtensions
    {
        private static MethodInfo ToListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));


        public static string ToJson(this IQueryable source)
        {
            if (source.Provider is ALinqlContext linqlProvider)
            {
                LinqlSearch search = source.ToLinqlSearch();
                string result = linqlProvider.ToJson(search);

                return result;
            }
            else
            {
                throw new UnsupportedIQueryableException();
            }
        }

        public static async Task<string> ToJsonAsync(this IQueryable source)
        {
            if (source.Provider is ALinqlContext linqlProvider)
            {
               LinqlSearch search = source.ToLinqlSearch();
                string result = await linqlProvider.ToJsonAsync(search);
                return result;
            }
            else
            {
                throw new UnsupportedIQueryableException();
            }
        }

        public static LinqlSearch ToLinqlSearch(this IQueryable source)
        {
            if (source.Provider is ALinqlContext linqlProvider)
            {
                LinqlSearch search = linqlProvider.BuildLinqlRequest(source.Expression, source.GetType().GetEnumerableType());
               
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
