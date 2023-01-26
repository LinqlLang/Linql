using Linql.Client.Internal;
using Linql.Core.Test;
using System.Linq.Expressions;

namespace Linql.Client.Test
{
    public class LinqlProviderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ProviderTests()
        {
            try
            {
                LinqlProvider provider = new CustomLinqlProvider(typeof(DataModel));
                object test = provider.CreateQuery(Expression.Constant(true));
                Assert.IsNull(test);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
                return;
            }


        }

        [Test]
        public void ProviderTests2()
        {
            try
            {
                LinqlProvider provider = new CustomLinqlProvider(typeof(DataModel));
                object test = provider.CreateQuery(Expression.Constant(true));
                Assert.IsNull(test);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
                return;
            }


        }


        [Test]
        public void ProviderTests3()
        {
            try
            {
                LinqlProvider provider = new CustomLinqlProvider(typeof(DataModel));
                object test = provider.Execute(Expression.Constant(true));
                Assert.IsNull(test);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
                return;
            }


        }

        [Test]
        public void ProviderTests4()
        {
            try
            {
                LinqlProvider provider = new CustomLinqlProvider(typeof(DataModel));
                bool test = provider.Execute<bool>(Expression.Constant(true));
                Assert.IsFalse(test);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
                return;
            }


        }
    }

    internal class CustomLinqlProvider : LinqlProvider
    {
        public CustomLinqlProvider(Type RootType) : base(RootType)
        {
            this.JsonOptions = new System.Text.Json.JsonSerializerOptions() { WriteIndented = true };
        }

    }


}