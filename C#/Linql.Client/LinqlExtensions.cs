using Linql.Client.Exception;
using Linql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linql.Client
{
    public static class LinqlExtensions
    {
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
                throw new UnsupportedExtensionProvider(source.Provider);
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
                throw new UnsupportedExtensionProvider(source.Provider);
            }
        }

        public static LinqlSearch ToLinqlSearch(this IQueryable source)
        {
            ALinqlContext context;
            if (source.Provider is ALinqlContext linqlProvider)
            {
                context = linqlProvider;
               
            }
            else
            {
                context = new LinqlContext();
            }

            LinqlSearch search = context.BuildLinqlRequest(source.Expression, source.GetType().GetEnumerableType());

            return search;
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            if (source.Provider is ALinqlContext linqlProvider)
            {
                LinqlSearch search = linqlProvider.BuildLinqlRequest(source.Expression, source.GetType().GetEnumerableType());

                LinqlExpression expression = search.Expressions.FirstOrDefault();
                LinqlExpression lastExpression = expression.GetLastExpressionInNextChain();

                lastExpression.Next = new LinqlFunction("ToListAsync");

                return await linqlProvider.SendRequestAsync<List<T>>(typeof(T), search);
            }
            else
            {
                throw new UnsupportedExtensionProvider(source.Provider);
            }
        }


    }
}
