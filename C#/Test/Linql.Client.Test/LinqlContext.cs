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

        [Test]
        public async Task TestJsonOptions()
        {
            //List<int> integers = new List<int>() { 1, 2, 3 };
            //LinqlSearch<DataModel> search = Context.Set<DataModel>();

            //string simpleConstant = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
            //this.TestLoader.Compare(nameof(Smoke.ListIntFromProperty), simpleConstant);
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