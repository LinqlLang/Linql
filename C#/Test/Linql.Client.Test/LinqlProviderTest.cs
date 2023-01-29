using Linql.Client.Internal;
using Linql.Core.Test;
using System.Linq.Expressions;

namespace Linql.Client.Test
{
    public class LinqlProviderTests
    {
        protected LinqlProvider provider { get; set; } = new CustomLinqlProvider();

        [OneTimeSetUp]
        public void Setup()
        {
        }

        [Test]
        public void ProviderTests()
        {
            try
            {
                
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
        public CustomLinqlProvider() : base()
        {
            this.JsonOptions = new System.Text.Json.JsonSerializerOptions() { WriteIndented = true };
        }

    }


}