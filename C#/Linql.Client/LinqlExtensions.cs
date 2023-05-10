using Linql.Client.Exception;
using Linql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Linql.Client
{
    /// <summary>
    /// LinqlExtensions that mirror System.Linq
    /// </summary>
    public static class LinqlExtensions
    {
        /// <summary>
        /// Returns a json representation of the IQueryable.  The IQueryable must have a LinqlContext as its provider
        /// </summary>
        /// <param name="source">The IQueryable</param>
        /// <returns>A Json string</returns>
        /// <exception cref="UnsupportedExtensionProvider">Throws if the provider is not of type ALinqlContext</exception>
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

        /// <summary>
        /// Returns a json representation of the IQueryable.  The IQueryable must have a LinqlContext as its provider
        /// </summary>
        /// <param name="source">The IQueryable</param>
        /// <returns>A Json string</returns>
        /// <exception cref="UnsupportedExtensionProvider">Throws if the provider is not of type ALinqlContext</exception>
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

        /// <summary>
        /// Turns an IQueryable into a LinqlSearch.  If the source does not have ALinqlContext as a provider, we create the default provider and use that.
        /// </summary>
        /// <param name="source">The IQueryable</param>
        /// <returns>A LinqlSearch</returns>
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

        /// <summary>
        /// Returns a LinqlSearch with a custom function appended ot it
        /// </summary>
        /// <param name="source">The IQueryable to operate on</param>
        /// <param name="FunctionName">The name of the Function</param>
        /// <param name="Predicate">The predicate of the function, e.g. r => true </param>
        /// <returns>A new LinqlSearch with the custom function appended to the expression tree</returns>
        /// <exception cref="UnsupportedExtensionProvider">Throws if the IQueryable does not have a ALinqlContext of its provider.</exception>
        public static LinqlSearch CustomLinqlFunction(this IQueryable source, string FunctionName, Expression Predicate = null)
        {
            LinqlSearch search = null;
            if (source.Provider is ALinqlContext linqlProvider)
            {
                search = linqlProvider.BuildLinqlRequest(source.Expression, source.GetType().GetEnumerableType());

                LinqlExpression expression = null;

                if (search.Expressions != null)
                {
                    expression = search.Expressions.FirstOrDefault();
                }

                LinqlFunction customFunction = new LinqlFunction(FunctionName);

                if (expression != null)
                {
                    LinqlExpression lastExpression = expression.GetLastExpressionInNextChain();
                    lastExpression.Next = customFunction;
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

        /// <summary>
        /// Executes a Custom Linql Function.  Used to denote that a CustomLinqlFunction materializes a LinqlSearch
        /// </summary>
        /// <param name="source">The IQueryable to operate on</param>
        /// <param name="FunctionName">The name of the Function</param>
        /// <param name="Predicate">The predicate of the function, e.g. r => true </param>
        /// <returns>The result of the LinqlSearch</returns>
        /// <exception cref="UnsupportedExtensionProvider">Throws if the IQueryable does not have a ALinqlContext of its provider.</exception>

        public static async Task<TResult> ExecuteCustomLinqlFunction<TSource, TResult>(this IQueryable<TSource> source, string FunctionName, Expression Predicate = null)
        {
            if (source.Provider is ALinqlContext linqlProvider)
            {
                LinqlSearch search = source.CustomLinqlFunction(FunctionName, Predicate);

                return await linqlProvider.SendRequestAsync<TResult>(search);
            }
            else
            {
                throw new UnsupportedExtensionProvider(source.Provider);
            }
        }

        /// <summary>
        /// Returns a LinqlSearch with ToListAsync appended to it.
        /// </summary>
        /// <param name="source">The IQueryable</param>
        /// <returns>A new LinqlSearch with ToListAsync appended to it</returns>
        public static LinqlSearch ToListAsyncSearch(this IQueryable source)
        {
            return source.CustomLinqlFunction("ToListAsync");
        }

        /// <summary>
        /// Returns a LinqlSearch with FirstOrDefaultAsync appended to it.
        /// </summary>
        /// <param name="source">The IQueryable</param>
        /// <returns>A new LinqlSearch with ToListAsync appended to it</returns>
        public static LinqlSearch FirstOrDefaultAsyncSearch<T>(this IQueryable<T> source, Expression<Func<T, bool>> Predicate = null)
        {
            return source.CustomLinqlFunction("FirstOrDefaultAsync", Predicate);
        }

        /// <summary>
        /// Returns a LinqlSearch with LastOrDefaultAsync appended to it.
        /// </summary>
        /// <param name="source">The IQueryable</param>
        /// <returns>A new LinqlSearch with ToListAsync appended to it</returns>
        public static LinqlSearch LastOrDefaultAsyncSearch<T>(this IQueryable<T> source, Expression<Func<T, bool>> Predicate = null)
        {
            return source.CustomLinqlFunction("LastOrDefaultAsync", Predicate);
        }

        ///// <summary>
        ///// Returns a LinqlSearch with MinAsync appended to it.
        ///// </summary>
        ///// <param name="source">The IQueryable</param>
        ///// <returns>A new LinqlSearch with ToListAsync appended to it</returns>
        ///// 



        /// <summary>
        /// Returns a LinqlSearch with MinAsync appended to it.
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable source</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="source">The IQueryable</param>
        /// <param name="Predicate">The predicate</param>
        /// <returns>A LinqlSearch with MinAsync appended to it</returns>
        public static LinqlSearch MinAsyncSearch<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> Predicate = null)
        {
            return source.CustomLinqlFunction("MinAsync", Predicate);
        }

        /// <summary>
        /// Returns a LinqlSearch with MaxAsync appended to it.
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable source</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="source">The IQueryable</param>
        /// <param name="Predicate">The predicate</param>
        /// <returns>A LinqlSearch with MaxAsync appended to it</returns>
        public static LinqlSearch MaxAsyncSearch<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> Predicate = null)
        {
            return source.CustomLinqlFunction("MaxAsync", Predicate);
        }

        /// <summary>
        /// Returns a List of type T.  Materializes the LinqlSearch.
        /// </summary>
        /// <typeparam name="T">The type of the List</typeparam>
        /// <param name="source">The IQueryable</param>
        /// <returns>A List of Type T</returns>
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            return await source.ExecuteCustomLinqlFunction<T, List<T>>("ToListAsync");
        }

        /// <summary>
        /// Returns the first item that matches the predicate, or null.  Materializes the LinqlSearch.
        /// </summary>
        /// <typeparam name="T">The type of the Result</typeparam>
        /// <param name="source">The IQueryable</param>
        /// <returns>A result of T or null</returns>

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> Predicate = null)
        {
            return await source.ExecuteCustomLinqlFunction<T, T>("FirstOrDefaultAsync", Predicate);
        }

        /// <summary>
        /// Returns the last item that matches the predicate, or null.  Materializes the LinqlSearch.
        /// </summary>
        /// <typeparam name="T">The type of the Result</typeparam>
        /// <param name="source">The IQueryable</param>
        /// <returns>A result of T or null</returns>

        public static async Task<T> LastOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> Predicate = null)
        {
            return await source.ExecuteCustomLinqlFunction<T, T>("LastOrDefaultAsync", Predicate);
        }


        /// <summary>
        /// Executes a MinAsync request on the IQueryable
        /// </summary>
        /// <typeparam name="T">The type of the source</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="source">The IQueryable</param>
        /// <param name="Predicate">An optional predicate</param>
        /// <returns>The Min TResult in the IQueryable</returns>
        public static async Task<TResult> MinAsync<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> Predicate = null)
        {
            return await source.ExecuteCustomLinqlFunction<T, TResult>("MinAsync", Predicate);
        }

        /// <summary>
        /// Executes a MaxAsync request on the IQueryable
        /// </summary>
        /// <typeparam name="T">The type of the source</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="source">The IQueryable</param>
        /// <param name="Predicate">An optional predicate</param>
        /// <returns>The Max TResult in the IQueryable</returns>

        public static async Task<TResult> MaxAsync<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> Predicate = null)
        {
            return await source.ExecuteCustomLinqlFunction<T, TResult>("MaxAsync", Predicate);
        }

    }
}
