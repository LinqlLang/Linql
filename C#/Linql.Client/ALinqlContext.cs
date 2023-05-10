using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using Linql.Core;
using System.Threading.Tasks;

namespace Linql.Client
{
    /// <summary>
    /// Abstract implementation of a LinqlContext.  LinqlContext implements IQueryProvider.  The QueryProviders have a recursive dependency on its Query Object.  
    /// </summary>
    public abstract class ALinqlContext : IQueryProvider
    {

        //#region IQueryProvider
        public IQueryable CreateQuery(Expression expression)
        {
            return default(IQueryable);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new LinqlSearch<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return default(object);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return default(TResult);
        }

        //$endregion

        //#region Context
        /// <summary>
        /// Returns the endpoint (url).  The Default implementation returns the url linql/{Search.Type.TypeName}
        /// </summary>
        /// <param name="Search">The LinqlSearch</param>
        /// <returns>Returns the linql endpoint url</returns>
        protected virtual string GetEndpoint(LinqlSearch Search)
        {
            return $"linql/{Search.Type.TypeName}";
        }

        /// <summary>
        /// Returns a new LinqlSearch that is already configured with this LinqlContext as the provider.
        /// </summary>
        /// <typeparam name="T">The Enumerable Type</typeparam>
        /// <returns>A LinqlSearch of type T, configured with this LinqlContext as a provider</returns>
        public virtual LinqlSearch<T> Set<T>()
        {
            return new LinqlSearch<T>(this);
        }

        /// <summary>
        /// Builds a LinqlSearch from a C# Expression Tree.  The RootType defines the entry point of the Search.
        /// This method is required to be public because of the recursive nature of IQueryProviders and their corresponding IQueryables.  This method should almost never be used or overridden.
        /// </summary>
        /// <param name="expression">The C# Expression</param>
        /// <param name="RootType">The entry point of the LinqlSearch</param>
        /// <returns>A nongeneric LinqlSearch</returns>
        public virtual LinqlSearch BuildLinqlRequest(Expression expression, Type RootType)
        {
           
            LinqlParser parser = new LinqlParser(expression);

            Type rootType = RootType;

            if(parser.LinqlSearchRootType != null)
            {
                rootType = parser.LinqlSearchRootType;
            }

            LinqlSearch search = new LinqlSearch(rootType);

            LinqlExpression root = parser.Root;

            if (root != null)
            {
                if (search.Expressions == null)
                {
                    search.Expressions = new List<LinqlExpression>();
                }
                search.Expressions.Add(root);
            }

            return search;
        }

        /// <summary>
        /// Called when an IQueryable is enumerated.  While Linql does support this, this method should almost never be used.  
        /// </summary>
        /// <typeparam name="TResult">The return type of the executed IQueryable Expression tree</typeparam>
        /// <param name="LinqlSearch">The LinqlSearch</param>
        /// <returns>The results of the executed IQueryable expression tree</returns>
        public abstract TResult SendRequest<TResult>(IQueryable LinqlSearch);

        /// <summary>
        /// Executes a LinqlSearch.  Called by any statement that materializes the IQueryable (ToList, ToListAsync, FirstOrDefault, FirstOrDefaultAsync, etc).
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="LinqlSearch">The LinqlSearch to execute</param>
        /// <returns>The result of the LinqlSearch</returns>
        public abstract Task<TResult> SendRequestAsync<TResult>(LinqlSearch LinqlSearch);

        /// <summary>
        /// Turns a LinqlSearch into Json.  This method is helpful if you wish to override the Json Serializer that Linql uses.
        /// </summary>
        /// <param name="Search">The LinqlSearch to serialize.</param>
        /// <returns>A json string</returns>
        public abstract string ToJson(LinqlSearch Search);

        /// <summary>
        /// Turns a LinqlSearch into Json.  This method is helpful if you wish to override the Json Serializer that Linql uses.
        /// </summary>
        /// <param name="Search">The LinqlSearch to serialize.</param>
        /// <returns>A json string</returns>
        public abstract Task<string> ToJsonAsync(LinqlSearch Search);

    }
}
