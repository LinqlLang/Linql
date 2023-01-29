using Linql.Core;
using System.Linq;
using System.Linq.Expressions;

namespace Linql.Core.Test
{
    public class TypeExtensionTests
    {
        [Test]
        public void IsNullable()
        {
            Type nullableType = typeof(Nullable<int>);
            Type notNullableType = typeof(int);

            Assert.True(nullableType.IsNullable());
            Assert.False(notNullableType.IsNullable());

        }

        [Test]
        public void GetGenericTypeDefinitionSafe()
        {
            Type nullableType = typeof(Nullable<int>);
            Type notNullableType = typeof(int);

            Assert.That(nullableType.GetGenericTypeDefinitionSafe(), Is.EqualTo(typeof(Nullable<>)));
            Assert.That(notNullableType.GetGenericTypeDefinitionSafe(), Is.EqualTo(typeof(int)));

        }

        [Test]
        public void IsEnumerable()
        {
            Type enumerable = typeof(IEnumerable<int>);
            Type list = typeof(List<int>);
            Type iqueryable = typeof(IQueryable<int>);

            Type notEnumerable = typeof(int);

            Assert.True(enumerable.IsEnumerable());
            Assert.True(list.IsEnumerable());
            Assert.True(iqueryable.IsEnumerable());

            Assert.False(notEnumerable.IsNullable());

        }

        [Test]
        public void GetEnumerableType()
        {
            Type enumerable = typeof(IEnumerable<int>);
            Type list = typeof(List<int>);
            Type iqueryable = typeof(IQueryable<int>);

            Type notEnumerable = typeof(int);

            Assert.That(enumerable.GetEnumerableType(), Is.EqualTo(typeof(int)));
            Assert.That(list.GetEnumerableType(), Is.EqualTo(typeof(int)));
            Assert.That(iqueryable.GetEnumerableType(), Is.EqualTo(typeof(int)));
            Assert.That(notEnumerable.GetEnumerableType(), Is.EqualTo(typeof(int)));


        }


        [Test]
        public void IsFunc()
        {
            Type func1 = typeof(Func<>);
            Type func2 = typeof(Func<,>);
            Type funct3 = typeof(Func<int>);
            Type func4 = typeof(Func<int, int>);
            
            Type notFunc = typeof(IQueryable<int>);

            Type notEnumerable = typeof(int);

            Assert.True(func1.IsFunc());
            Assert.True(func2.IsFunc());
            Assert.True(funct3.IsFunc());
            Assert.True(func4.IsFunc());

            Assert.False(notFunc.IsFunc());
            Assert.False(notEnumerable.IsFunc());


        }

        [Test]
        public void IsExpression()
        {
            Type exp1 = typeof(Expression);
            Type exp2 = typeof(BinaryExpression);
            Type exp3 = typeof(LambdaExpression);
            Type exp4 = typeof(UnaryExpression);

            Type notExp = typeof(IQueryable<int>);

        
            Assert.True(exp1.IsExpression());
            Assert.True(exp2.IsExpression());
            Assert.True(exp3.IsExpression());
            Assert.True(exp4.IsExpression());

            Assert.False(notExp.IsExpression());


        }

    }

}