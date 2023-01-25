using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Linql.Client.Json;
using System.Data.Common;
using System.Xml.Linq;

namespace Linql.Client.Internal
{
    public partial class LinqlProvider : ExpressionVisitor, IQueryProvider
    {

        protected Type RootType { get; set; }

        protected Linql.Client.Json.LinqlSearch Search { get; set; }

        public JsonSerializerOptions JsonOptions { get; protected set; } = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };

        public LinqlProvider(Type RootType)
        {
            this.RootType = RootType;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return default(IQueryable);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new LinqlSearch<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return default(TResult);
        }

        public virtual Linql.Client.Json.LinqlSearch BuildLinqlRequest(Expression expression)
        {
            this.Search = new Linql.Client.Json.LinqlSearch(this.RootType);
            LinqlParser parser = new LinqlParser(expression);
            LinqlExpression root = parser.Root;

            if (!(root is LinqlConstant constant && constant.ConstantType.TypeName == nameof(LinqlSearch)))
            {
                if (root != null)
                {
                    if (this.Search.Expressions == null)
                    {
                        this.Search.Expressions = new List<LinqlExpression>();
                    }
                    this.Search.Expressions.Add(root);
                }
            }
            return this.Search;
        }

    }
}
