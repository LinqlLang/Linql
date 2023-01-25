using Linql.Client.Internal;
using System.Collections;

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
    }
}