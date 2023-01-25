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
        public LinqlSearch() : base(typeof(T))
        {
            this.Provider = new LinqlProvider(typeof(T));
            Expression = Expression.Constant(this);
        }
        public LinqlSearch(LinqlContext Context) : base(typeof(T))
        {
            this.Provider = new LinqlProvider(typeof(T));
            Expression = Expression.Constant(this);
        }

        internal LinqlSearch(IQueryProvider provider, Expression expression) : base(typeof(T))
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
