using Linql.Client.Exception;
using Linql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static LinqlSearch CustomLinqlFunction(this IQueryable source, string FunctionName, Expression Predicate = null)
        {
            LinqlSearch search = null;
            if (source.Provider is ALinqlContext linqlProvider)
            {
                search = linqlProvider.BuildLinqlRequest(source.Expression, source.GetType().GetEnumerableType());

                LinqlExpression expression = search.Expressions?.FirstOrDefault();
                LinqlFunction customFunction = new LinqlFunction(FunctionName);

                if (expression != null)
                {
                    LinqlExpression lastExpression = expression.GetLastExpressionInNextChain();
                    lastExpression.Next = customFunction;
                }
                else
                {
                    search.Expressions = new List<LinqlExpression>();
                    search.Expressions.Add(customFunction);
                }

                if (Predicate != null) 
                {
                    LinqlParser parser = new LinqlParser(Predicate);
                    customFunction.Arguments = new List<LinqlExpression>() { parser.Root };
                }

              
            }
            else
            {
                throw new UnsupportedExtensionProvider(source.Provider);
            }

            return search;

        }

        public static async Task<TResult> ExecuteCustomLinqlFunction<TSource, TResult>(this IQueryable<TSource> source, string FunctionName, Expression Predicate = null)
        {
            if (source.Provider is ALinqlContext linqlProvider)
            {
                LinqlSearch search = source.CustomLinqlFunction(FunctionName, Predicate);

                return await linqlProvider.SendRequestAsync<TResult>(typeof(TSource), search);
            }
            else
            {
                throw new UnsupportedExtensionProvider(source.Provider);
            }
        }

        public static LinqlSearch ToListAsyncSearch(this IQueryable source)
        {
            return source.CustomLinqlFunction("ToListAsync");
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            return await source.ExecuteCustomLinqlFunction<T, List<T>>("ToListAsync");
        }

        public static LinqlSearch FirstOrDefaultAsyncSearch<T>(this IQueryable<T> source, Expression<Func<T, bool>> Predicate = null)
        {
            return source.CustomLinqlFunction("FirstOrDefaultAsync", Predicate);
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> Predicate = null)
        {
            return await source.ExecuteCustomLinqlFunction<T, T>("FirstOrDefaultAsync", Predicate);
        }


    }
}
