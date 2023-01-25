using Linql.Client.Internal;
using Linql.Client.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Linql.Client
{
    public class LinqlSearch<T> : LinqlSearch, IQueryable<T>
    {
        protected LinqlContext Context { get; set; }

        public LinqlSearch() : this(null, null, null) { }

        public LinqlSearch(LinqlContext Context = null, IQueryProvider QueryProvider = null, Expression Expression = null) : base(typeof(T))
        {
            if(Context == null)
            {
                this.Context = new LinqlContext();
            }
            if(QueryProvider == null)
            {
                this.Provider = new LinqlProvider(typeof(T));
            }
            this.Expression = Expression;
        }

        internal LinqlSearch(IQueryProvider provider, Expression expression) : this(null, provider, expression)
        {
            this.Provider = provider;
            this.Expression = expression;
        }


        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            throw new EnumerationIsNotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new EnumerationIsNotSupportedException();
        }
    }
}
