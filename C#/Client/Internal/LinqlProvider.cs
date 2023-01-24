using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Linql.Client.Json;
using System.Data.Common;

namespace Linql.Client.Internal
{
    public partial class LinqlProvider : ExpressionVisitor, IQueryProvider
    {

        protected Type RootType { get; set; }

        protected Linql.Client.Json.LinqlSearch Search { get; set; }

        protected Stack<LinqlExpression> LinqlStack { get; set; } = new Stack<LinqlExpression>();

        protected Stack<Expression> ExpressionStack { get; set; } = new Stack<Expression>();

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
            return null;
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
            return default(TResult); // (TResult)LambdaSearchQueryContext<TResult>.Execute(expression);
        }

        public Linql.Client.Json.LinqlSearch BuildLinqlRequest(Expression expression)
        {
            this.Search = new Linql.Client.Json.LinqlSearch(this.RootType.Name);
            this.Visit(expression);
            return this.Search;
        }

        //public override Expression Visit(Expression expression)
        //{
        //    return base.Visit(expression);
        //}

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (!(c.Value is Linql.Client.Json.LinqlSearch))
            {

            }
            return base.VisitConstant(c);
        }

        protected void PushToStack(LinqlExpression LinqlExpression, Expression CSharpExpression)
        {
            this.LinqlStack.Push(LinqlExpression);
            this.ExpressionStack.Push(CSharpExpression);
        }

        protected void PopStack()
        {
            this.LinqlStack.Pop();
            this.ExpressionStack.Pop();
        }


        protected override Expression VisitMethodCall(MethodCallExpression m)
        {

            LinqlFunction function = new LinqlFunction(m.Method.Name);

            if(this.LinqlStack.Count == 0)
            {
                
                if(this.Search.Expressions == null)
                {
                    this.Search.Expressions = new List<LinqlExpression>();
                }
                this.Search.Expressions.Add(function);
            }

            this.PushToStack(function, m);

            Expression expression = base.VisitMethodCall(m);

            this.PopStack();

            return expression;
        }

        //protected override Expression VisitUnary(UnaryExpression t)
        //{
        //    return base.VisitUnary(t);
        //}

        //protected override Expression VisitLambda<T>(LambdaExpression lambda)
        //{
        //    return base.VisitLambda<T>(lambda);
        //}

        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            LinqlExpression previous = this.LinqlStack.First();
            LinqlLambda linqlLambda = new LinqlLambda();

            Expression expression = base.VisitLambda<T>(lambda);

            this.AttachToExpression(previous, linqlLambda);
            this.PopStack();

            return expression;
        }


        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            LinqlExpression previous = this.LinqlStack.First();

            LinqlParameter param = new LinqlParameter(parameter.Name);
            this.PushToStack(param, parameter);
            Expression expression = base.VisitParameter(parameter);
            this.AttachToExpression(previous, param);
            return expression;
        }

        protected void AttachToExpression(LinqlExpression PreviousExpression, LinqlExpression ExpressionToAttach)
        {
            if(PreviousExpression is LinqlLambda lambda)
            {
                lambda.Body = ExpressionToAttach;
            }
            else
            {
                PreviousExpression.Next = ExpressionToAttach;
            }
        }

        protected override Expression VisitMember(MemberExpression m)
        {

            LinqlProperty property = new LinqlProperty(m.Member.Name);

            base.VisitMember(m);
            LinqlExpression previous = this.LinqlStack.First();

            this.AttachToExpression(previous, property);

            this.PushToStack(property, m);


            return m;
        }

        //protected override Expression VisitConstant(ConstantExpression c)
        //{

        //    return c;

        //}

        //public ALambdaSearchRequest ExecuteSearch<T>(Expression expression)
        //{
        //    return LambdaSearchQueryContext<T>.Execute(expression);
        //}
    }
}
