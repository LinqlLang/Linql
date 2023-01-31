using System.Linq.Expressions;

namespace Linql.Client.Test
{
    public class ALinqlContext_Test
    {
        [OneTimeSetUp]
        public void Setup()
        {
        }

        //[Test]
        //public void ConstructorEmpty()
        //{

        //    Assert.DoesNotThrowAsync(async () =>
        //    {
        //        LinqlProvider provider = new LinqlProvider();
        //        LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(provider);
        //        string json = await search.Where(r => true).ToJsonAsync();

        //        Assert.False(json.Contains(Environment.NewLine));

        //    });

        //}

        //[Test]
        //public void ConstructorNotEmpty()
        //{

        //    Assert.DoesNotThrowAsync(async () =>
        //    {
        //        LinqlProvider provider = new LinqlProvider(new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
        //        LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(provider);
        //        string json = await search.Where(r => true).ToJsonAsync();

        //        Assert.True(json.Contains(Environment.NewLine));

        //    });

        //}


        [Test]
        public void CreateQuery()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlContext context = new LinqlContext();
                object test = context.CreateQuery(Expression.Constant(true));
                Assert.IsNull(test);
            });

        }

        [Test]
        public void Execute()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlContext context = new LinqlContext();
                object test = context.Execute(Expression.Constant(true));
                Assert.IsNull(test);
            });
        }

        [Test]
        public void ExecuteGeneric()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlContext context = new LinqlContext();
                bool test = context.Execute<bool>(Expression.Constant(true));
                Assert.IsFalse(test);
            });

        }
    }
}