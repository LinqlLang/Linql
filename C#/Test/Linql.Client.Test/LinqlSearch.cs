using Linql.Client.Internal;
using System.Collections;
using System.Linq.Expressions;

namespace Linql.Client.Test
{
    public class LinqlSearch
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            Assert.That(search.Type.TypeName, Is.EqualTo("DataModel"));
        }

        [Test]
        public void Constructor2()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            Assert.That(search.Type.TypeName, Is.EqualTo("DataModel"));
        }

        [Test]
        public void GetEnumerator()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();

            try
            {
                foreach (DataModel item in search)
                {

                }
            }
            catch (EnumerationIsNotSupportedException ex)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);
        }

        [Test]
        public void GetEnumerator2()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();

            try
            {
                IEnumerable cast = (IEnumerable) search;
                cast.GetEnumerator();
            }
            catch (EnumerationIsNotSupportedException ex)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);
        }

        [Test]
        public void ElementType()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            Assert.That(search.ElementType, Is.EqualTo(typeof(DataModel)));
        }


        [Test]
        public void CustomProvider()
        {
            CustomLinqlSearch<DataModel> search = new CustomLinqlSearch<DataModel>();
            Assert.That(search.Type.TypeName, Is.EqualTo("DataModel"));
        }

        [Test]
        public async Task TestJsonOptions()
        {
            List<int> integers = new List<int>() { 1, 2, 3 };
            CustomLinqlSearch<DataModel> search = new CustomLinqlSearch<DataModel>();

            string simpleConstant = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
            
        }

        [Test]
        public void NullProvider()
        {
            try
            {
                LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(null);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);

        }

        [Test]
        public void NullExpression()
        {
            try
            {
                LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(new CustomLinqlProvider(typeof(DataModel)), null);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);

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

    internal class CustomLinqlSearch<T> : LinqlSearch<T>
    {
        public CustomLinqlSearch() : base(new CustomLinqlProvider(typeof(T))) { }
    }
}