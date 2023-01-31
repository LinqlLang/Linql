//using Linql.Client.Internal;
//using Linql.Core.Test;
//using System.Linq.Expressions;

//namespace Linql.Client.Test
//{
//    public class LinqlProviderTests
//    {
//        protected LinqlProvider provider { get; set; } = new CustomLinqlProvider();

//        [OneTimeSetUp]
//        public void Setup()
//        {
//        }

//        [Test]
//        public void ConstructorEmpty()
//        {

//            Assert.DoesNotThrowAsync(async () =>
//            {
//                LinqlProvider provider = new LinqlProvider();
//                LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(provider);
//                string json = await search.Where(r => true).ToJsonAsync();

//                Assert.False(json.Contains(Environment.NewLine));

//            });

//        }

//        [Test]
//        public void ConstructorNotEmpty()
//        {

//            Assert.DoesNotThrowAsync(async () =>
//            {
//                LinqlProvider provider = new LinqlProvider(new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
//                LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(provider);
//                string json = await search.Where(r => true).ToJsonAsync();

//                Assert.True(json.Contains(Environment.NewLine));

//            });

//        }


//        [Test]
//        public void CreateQuery()
//        {
//            Assert.DoesNotThrow(() =>
//            {
//                object test = provider.CreateQuery(Expression.Constant(true));
//                Assert.IsNull(test);
//            });
            
//        }

//        [Test]
//        public void Execute()
//        {
//            Assert.DoesNotThrow(() =>
//            {
//                object test = provider.Execute(Expression.Constant(true));
//                Assert.IsNull(test);
//            });           
//        }

//        [Test]
//        public void ExecuteGeneric()
//        {
//            Assert.DoesNotThrow(() =>
//            {
//                bool test = provider.Execute<bool>(Expression.Constant(true));
//                Assert.IsFalse(test);
//            });
           
//        }
//    }


//    internal class CustomLinqlProvider : LinqlProvider
//    {
//        public CustomLinqlProvider() : base()
//        {
//            this.JsonOptions = new System.Text.Json.JsonSerializerOptions() { WriteIndented = true };
//        }

//    }


//}