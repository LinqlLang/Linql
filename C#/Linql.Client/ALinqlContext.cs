using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using Linql.Core;
using System.Threading.Tasks;

namespace Linql.Client
{
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

        protected virtual string GetEndpoint(Type QueryableType, LinqlSearch Search)
        {

            return $"linql/{Search.Type.TypeName}";
        }

        public virtual LinqlSearch<T> Set<T>()
        {
            return new LinqlSearch<T>(this);
        }

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

        public abstract TResult SendRequest<TResult>(IQueryable LinqlSearch);

        public abstract Task<TResult> SendRequestAsync<TResult>(Type Type, LinqlSearch LinqlSearch);

        public abstract string ToJson(LinqlSearch Search);

        public abstract Task<string> ToJsonAsync(LinqlSearch Search);

    }
}
