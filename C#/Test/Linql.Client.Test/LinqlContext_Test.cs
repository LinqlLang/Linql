using Linql.Core.Test;
using RichardSzalay.MockHttp;
using System.Collections;
using System.Text;

namespace Linql.Client.Test
{
    public class LinqlContext_Test
    {
        [OneTimeSetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor_Without_BaseUrl()
        {
            LinqlContext context = new LinqlContext();
            Assert.IsNull(context.BaseUrl);
        }

        [Test]
        public void Constructor_With_BaseUrl()
        {
            LinqlContext context = new LinqlContext("http://localhost");
            Assert.IsNotNull(context.BaseUrl);
        }

        [Test]
        public void Constructor_With_JsonOptions()
        {
            LinqlContext context = new LinqlContext("http://localhost", new JsonSerializerOptions() { WriteIndented = true });
            string json = context.ToJson(new LinqlSearch());
            Assert.True(json.Contains(Environment.NewLine));
        }

        [Test]
        public void Constructor_Without_JsonOptions()
        {
            LinqlContext context = new LinqlContext("http://localhost");
            string json = context.ToJson(new LinqlSearch());
            Assert.False(json.Contains(Environment.NewLine));
        }


        [Test]
        public async Task ToJsonAsync_Matches_ToJson()
        {
            LinqlContext context = new LinqlContext("http://localhost");
            string json = context.ToJson(new LinqlSearch());
            string asyncJson = await context.ToJsonAsync(new LinqlSearch());
            Assert.That(json, Is.EqualTo(asyncJson));
        }


        [Test]
        public async Task ToList_Does_Not_Throw_When_BaseUrl_Set()
        {

            Assert.DoesNotThrow(() =>
            {
                LinqlContext context = new ListMockLinqlContext("http://localhost");

                bool test = false;
                LinqlSearch<DataModel> search = context.Set<DataModel>();
                List<DataModel> output = search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToList();
            });
           
        }

        [Test]
        public async Task ToList_Should_Throw_When_BaseUrl_Set()
        {

            Assert.Catch(() =>
            {
                LinqlContext context = new ListMockLinqlContext();

                bool test = false;
                LinqlSearch<DataModel> search = context.Set<DataModel>();
                List<DataModel> output = search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToList();

                Assert.That(output.Count(), Is.EqualTo(0));
            });

        }

        [Test]
        public async Task ToList_Enumeration_Not_Supported_Except_for_LinqlSearches()
        {

            Assert.Catch(() =>
            {

                LinqlContext context = new ListMockLinqlContext();

                //bool test = false;
                LinqlSearch<DataModel> search = context.Set<DataModel>();
                IEnumerable enumerable = (IEnumerable) search.AsEnumerable();
                enumerable.GetEnumerator();
                //List<DataModel> output = search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToList();

                //Assert.That(output.Count(), Is.EqualTo(0));
            });

        }


        [Test]
        public async Task ToListAsync_Should_Work()
        {

            Assert.DoesNotThrowAsync(async () =>
            {
                LinqlContext context = new ListMockLinqlContext("http://localhost");

                bool test = false;
                LinqlSearch<DataModel> search = context.Set<DataModel>();
                List<DataModel> output = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToListAsync();

                Assert.That(output.Count(), Is.EqualTo(0));
            });

        }


        [Test]
        public async Task ToListAsync_IQueryable()
        {

            Assert.DoesNotThrowAsync(async () =>
            {
                LinqlContext context = new ListMockLinqlContext("http://localhost");

                bool test = false;
                LinqlSearch<DataModel> search = context.Set<DataModel>();
                List<DataModel> output = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToListAsync();

                Assert.That(output.Count(), Is.EqualTo(0));
            });

        }

        [Test]
        public async Task FirstOrDefaultAsync_IQueryable()
        {

            Assert.DoesNotThrowAsync(async () =>
            {
                LinqlContext context = new SingleMockLinqlContext("http://localhost");

                bool test = false;
                LinqlSearch<DataModel> search = context.Set<DataModel>();
                DataModel output = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).FirstOrDefaultAsync();

                Assert.That(output, Is.Not.EqualTo(null));
            });

        }


        [Test]
        public async Task LastOrDefaultAsync_IQueryable()
        {

            Assert.DoesNotThrowAsync(async () =>
            {
                LinqlContext context = new SingleMockLinqlContext("http://localhost");

                bool test = false;
                LinqlSearch<DataModel> search = context.Set<DataModel>();
                DataModel output = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).LastOrDefaultAsync();

                Assert.That(output, Is.Not.EqualTo(null));
            });

        }

        [Test]
        public void Search_Type_Based_On_First_Expression()
        {

            Assert.DoesNotThrow(() =>
            {
                ListMockLinqlContext context = new ListMockLinqlContext();

                LinqlSearch<DataModel> search = context.Set<DataModel>();
                LinqlSearch searchOne = search.ToListAsyncSearch();
                LinqlSearch searchTwo = search.SelectMany(r => r.ListNullableModel).ToListAsyncSearch();
                LinqlSearch searchThree = search.Select(r => r.OneToOneNullable).ToListAsyncSearch();

                string urlOne = context.GetRoute(searchOne);
                string urlTwo = context.GetRoute(searchTwo);
                string urlThree = context.GetRoute(searchThree);


                Assert.That(searchOne.Type, Is.EqualTo(searchTwo.Type));
                Assert.That(searchOne.Type, Is.EqualTo(searchThree.Type));
                Assert.That(urlOne, Is.EqualTo(urlTwo));
                Assert.That(urlOne, Is.EqualTo(urlThree));

            });

        }

        [Test]
        public void Custom_Function()
        {

            Assert.DoesNotThrow(() =>
            {
                ListMockLinqlContext context = new ListMockLinqlContext();

                LinqlSearch<DataModel> search = context.Set<DataModel>();
                LinqlSearch result = search.CustomLinqlFunction("Test");
                
                Assert.That(result.Expressions.Count, Is.EqualTo(1));

            });

        }

        [Test]
        public void CustomLinqlFunction_Wrong_Provider()
        {

            Assert.Catch(() =>
            {
                List<DataModel> test = new List<DataModel>();
                LinqlSearch result = test.AsQueryable().CustomLinqlFunction("Test");

                

            });

        }

    }


    class ListMockLinqlContext : LinqlContext
    {
        public ListMockLinqlContext() : base(null) { }
        public ListMockLinqlContext(string BaseUrl = null) : base(BaseUrl) { }


        public string GetRoute(LinqlSearch Search)
        {
            return this.GetEndpoint(Search);
        }

        protected virtual MockHttpMessageHandler BuildHandler()
        {
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When("http://localhost/linql/*")
                    .Respond("application/json", "[]");
            return mockHttp;
        }

        public override string BaseUrl
        {
            set
            {
                if(value == null)
                {
                    this.HttpClient = null;
                }
                else
                {
                    if (this.HttpClient == null)
                    {
                       
                        this.HttpClient = this.BuildHandler().ToHttpClient();
                    }
                    this.HttpClient.BaseAddress = new Uri(value);
                }
            }
        }
    }

    class SingleMockLinqlContext : ListMockLinqlContext
    {
        public SingleMockLinqlContext() : base(null) { }
        public SingleMockLinqlContext(string BaseUrl = null) : base(BaseUrl) { }


        protected override MockHttpMessageHandler BuildHandler()
        {
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When("http://localhost/linql/*")
                    .Respond("application/json", "{}");
            return mockHttp;
        }
    }
}