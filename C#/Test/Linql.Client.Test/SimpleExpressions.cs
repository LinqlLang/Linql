//using Linql.Client.Internal;
//using Linql.Core.Test;
//using Linql.Test.Files;
//using Microsoft.VisualStudio.TestPlatform.Utilities;
//using System.Text.Json;

//namespace Linql.Client.Test
//{
//    public class SimpleExpressions : TestFileTests
//    {
//        protected LinqlContext Context { get; set; } = new LinqlContext(null, new LinqlProviderPrettyPrint());

//        protected override string TestFolder { get; set; } = "./SimpleExpressions";

//        [Test]
//        public async Task LinqlConstant()
//        {
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => false).ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlConstant), output);
//        }

//        [Test]
//        public async Task LinqlBinary()
//        {
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => r.Boolean && r.OneToOne.Boolean).ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlBinary), output);
//        }

//        [Test]
//        public async Task LinqlUnary()
//        {
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => !r.Boolean).ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlUnary), output);
//        }

//        [Test]
//        public async Task LinqlObject()
//        {
//            LinqlObject<DataModel> objectTest = new LinqlObject<DataModel>(new DataModel());
//            Assert.That(objectTest.TypedValue, Is.EqualTo(objectTest.Value));
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => objectTest.TypedValue.Integer == r.Integer).ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlObject), output);

//        }

//        [Test]
//        public async Task LinqlFunction()
//        {
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlFunction), output);

//        }

//        [Test]
//        public async Task LinqlLambda()
//        {
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => r.ListInteger.Any(s => s > 0)).ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlLambda), output);

//        }


//        [Test]
//        public async Task FunctionChaining()
//        {
//            bool test = false;
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => false).Where(r => true).ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.FunctionChaining), output);
//        }

//        [Test]
//        public async Task NullableCheck()
//        {
//            bool test = false;
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.NullableCheck), output);
//        }

//        [Test]
//        public async Task ToList()
//        {
//            bool test = false;
//            LinqlSearch<DataModel> search = Context.Set<DataModel>();
//            string output = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToLinqlList().ToJsonAsync();
//            this.TestLoader.Compare(nameof(SimpleExpressions.ToList), output);
//        }

//        [Test]
//        public async Task ExecuteToList()
//        {
//            Assert.Catch(() =>
//            {
//                bool test = false;
//                LinqlSearch<DataModel> search = Context.Set<DataModel>();
//                List<DataModel> output = search.AsQueryable().ToList();
//            });
//        }
//    }
//}