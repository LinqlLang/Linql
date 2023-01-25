namespace Linql.Client.Test
{
    public class LinqlContextTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ConstructorWithoutBaseUrl()
        {
            LinqlContextDerived context = new LinqlContextDerived();
            Assert.IsNull(context.GetClient());
        }

        [Test]
        public void ConstructorWithBaseUrl()
        {
            LinqlContextDerived context = new LinqlContextDerived("http://localhost");
            Assert.IsNotNull(context.GetClient());
        }

     

    }

    internal class LinqlContextDerived : LinqlContext
    {
        public LinqlContextDerived(string BaseUrl = null) : base(BaseUrl) { }

        public HttpClient GetClient()
        {
            return this.HttpClient;
        }
    }
}