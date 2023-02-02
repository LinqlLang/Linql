using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test.Expressions
{
    public class Smoke_Test : TestFileTests
    {

        protected override string TestFolder { get; set; } = "Smoke";


        public IQueryable<DataModel> Data { get; set; }

        private LinqlCompiler Compiler = new LinqlCompiler(
            new HashSet<Assembly>()
            {
                typeof(System.Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly,
                typeof(DataModel).Assembly
            },
            true
            );


        [OneTimeSetUp]
        public override async Task Setup()
        {
            await base.Setup();
            List<DataModel> dataList = new List<DataModel>();
            dataList.Where(r => true).ToList();
            foreach (int index in Enumerable.Range(1, 100))
            {
                DataModel data = new CompiledDataModel(index, true);
                data.Integer = index;
                dataList.Add(data);
            }

            Data = dataList.AsQueryable();

        }

        private void TestFile(string FileName, int Count) 
        {
            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles[FileName];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                IEnumerable<DataModel> data = this.Compiler.Execute<IEnumerable<DataModel>>(search, this.Data);

                Assert.That(data.Count(), Is.EqualTo(Count));

            });
        }


        [Test]
        public void EmptySearch()
        {
            this.TestFile(nameof(EmptySearch), 100);
        }

        [Test]
        public void TwoBooleans()
        {
            this.TestFile(nameof(TwoBooleans), 0);
        }

        [Test]
        public void ThreeBooleans()
        {
            this.TestFile(nameof(ThreeBooleans), 50);
        }

        [Test]
        public void BooleanNegate()
        {
            this.TestFile(nameof(BooleanNegate), 50);
        }

        [Test]
        public void BooleanVar()
        {
            this.TestFile(nameof(BooleanVar), 100);
        }

        [Test]
        public void ComplexBoolean()
        {
            this.TestFile(nameof(ComplexBoolean), 0);
        }

        [Test]
        public void LinqlObject()
        {
            this.TestFile(nameof(LinqlObject), 0);
        }

        [Test]
        public void LinqlObject_NonZero()
        {
            this.TestFile(nameof(LinqlObject_NonZero), 1);
        }

        [Test]
        public void List_Int_Contains()
        {
            this.TestFile(nameof(List_Int_Contains), 3);
        }



        //[Test]
        //public async Task SimpleConstant()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => true).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.SimpleConstant), simpleConstant);
        //}

        //[Test]
        //public async Task SimpleBooleanProperty()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => r.Boolean).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.SimpleBooleanProperty), simpleConstant);
        //}

        //[Test]
        //public async Task BooleanNegate()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => !r.Boolean).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.BooleanNegate), simpleConstant);
        //}

        //[Test]
        //public async Task SimpleBooleanPropertyChaining()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => r.OneToOne.Boolean).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.SimpleBooleanPropertyChaining), simpleConstant);
        //}

        //[Test]
        //public async Task SimpleBooleanPropertyEquals()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => r.Boolean == false).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.SimpleBooleanPropertyEquals), simpleConstant);
        //}

        //[Test]
        //public async Task SimpleBooleanPropertyEqualsSwap()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => false == r.Boolean).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.SimpleBooleanPropertyEqualsSwap), simpleConstant);
        //}

        //[Test]
        //public async Task TwoBooleans()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => false == true).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.TwoBooleans), simpleConstant);
        //}

        //[Test]
        //public async Task BooleanVar()
        //{
        //    bool test = false;
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => false == test).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.BooleanVar), simpleConstant);
        //}

        //[Test]
        //public async Task ComplexBoolean()
        //{
        //    DataModel test = new DataModel();
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => test.Boolean).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.ComplexBoolean), simpleConstant);
        //}

        //[Test]
        //public async Task ComplexBooleanAsArgument()
        //{
        //    DataModel test = new DataModel();
        //    await InternalComplexBooleanAsArgument(test);

        //}

        //private async Task InternalComplexBooleanAsArgument(DataModel test)
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => test.Boolean).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.ComplexBoolean), simpleConstant);
        //}

        //[Test]
        //public async Task ThreeBooleans()
        //{
        //    DataModel test = new DataModel();
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => r.Boolean && r.Boolean && r.Boolean).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.ThreeBooleans), simpleConstant);
        //}

        //[Test]
        //public async Task ListInt()
        //{
        //    List<int> integers = new List<int>() { 1, 2, 3 };
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => integers.Contains(r.Integer)).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.ListInt), simpleConstant);
        //}


        //[Test]
        //public async Task ListIntFromProperty()
        //{
        //    List<int> integers = new List<int>() { 1, 2, 3 };
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.ListIntFromProperty), simpleConstant);
        //}

        //[Test]
        //public async Task InnerLambda()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => r.ListInteger.Any(s => s == 1)).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.InnerLambda), simpleConstant);
        //}

        //[Test]
        //public async Task NullableHasValue()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => r.OneToOneNullable.Integer.HasValue).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.NullableHasValue), simpleConstant);
        //}

        //[Test]
        //public async Task NullableValue()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.NullableValue), simpleConstant);
        //}

        //[Test]
        //public async Task LinqlObject()
        //{
        //    LinqlObject<DataModel> objectTest = new LinqlObject<DataModel>(new DataModel());
        //    Assert.That(objectTest.TypedValue, Is.EqualTo(objectTest.Value));
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => objectTest.TypedValue.Integer == r.Integer).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.LinqlObject), simpleConstant);
        //}

        //[Test]
        //public async Task ObjectCalculationWithNull()
        //{
        //    DataModel objectTest = new DataModel();
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => objectTest.OneToOne.Integer == r.Integer).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.ObjectCalculationWithNull), simpleConstant);
        //}

        //[Test]
        //public async Task ObjectCalculationWithoutNull()
        //{
        //    DataModel objectTest = new DataModel();
        //    objectTest.OneToOne = new DataModel();
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string simpleConstant = await search.Where(r => objectTest.OneToOne.Integer == r.Integer).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.ObjectCalculationWithoutNull), simpleConstant);
        //}

        //[Test]
        //public async Task List_Int_Contains()
        //{
        //    List<int> intList = new List<int>() { 1, 2, 3 };
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string contains = await search.Where(r => intList.Contains(r.Integer)).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.List_Int_Contains), contains);
        //}

        //[Test]
        //public async Task String_Contains()
        //{

        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string contains = await search.Where(r => "3".ToLowerInvariant().Contains(r.String)).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.String_Contains), contains);
        //}

        //[Test]
        //public async Task List_Int_Count()
        //{
        //    List<int> intList = new List<int>() { 1, 2, 3 };
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string contains = await search.Where(r => intList.Count == 1).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.List_Int_Count), contains);
        //}

        //[Test]
        //public async Task Inner_Lambda()
        //{

        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string contains = await search.Where(r => r.ListRecusrive.Any(s => s.ListInteger.Contains(1))).ToJsonAsync();
        //    TestLoader.Compare(nameof(Smoke_Test.Inner_Lambda), contains);
        //}
    }

}