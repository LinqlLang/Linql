namespace Linql.Client.Test
{
    public class LinqlSearch
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor1()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            Assert.That(search.Type.TypeName, Is.EqualTo("DataModel"));
        }
    }
}