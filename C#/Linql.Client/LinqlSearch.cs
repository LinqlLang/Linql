using Linql.Client.Internal;
using Linql.Core;
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

        public LinqlSearch(IQueryProvider Provider): base(typeof(T))
        {
            if(Provider == null)
            {
                throw new Exception("Provider cannot be null");
            }
           this.Provider = Provider;
            Expression = Expression.Constant(this);
        }


        public LinqlSearch(IQueryProvider Provider, Expression Expression) : this(Provider)
        {
            if(Expression == null)
            {
                throw new Exception("Expression cannot be null");
            }
            this.Expression = Expression;
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
