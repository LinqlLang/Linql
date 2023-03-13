using Linql.Core.Test;
using Linql.Test.Files;

namespace Linql.Client.Test.Expressions
{
    public class Smoke_Test : TestFileTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext();

        protected override string TestFolder { get; set; } = "Smoke";

        [Test]
        //Should fail.
        public void IncorrectToJson()
        {
            List<int> listType = new List<int>();

            try
            {
                listType.AsQueryable().ToJson();
            }
            catch (System.Exception ex)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);
        }

        [Test]
        //Should fail.
        public async Task IncorrectToJsonAsync()
        {
            List<int> listType = new List<int>();

            try
            {
                await listType.AsQueryable().ToJsonAsync();
            }
            catch (System.Exception ex)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);
        }


        [Test]
        public void EmptySearch()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string empty = search.ToJson();

            TestLoader.Compare(nameof(Smoke_Test.EmptySearch), empty);
        }

        [Test]
        public async Task SimpleConstant()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => true).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.SimpleConstant), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanProperty()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.SimpleBooleanProperty), simpleConstant);
        }

        [Test]
        public async Task BooleanNegate()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => !r.Boolean).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.BooleanNegate), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanPropertyChaining()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.OneToOne.Boolean).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.SimpleBooleanPropertyChaining), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanPropertyEquals()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean == false).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.SimpleBooleanPropertyEquals), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanPropertyEqualsSwap()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == r.Boolean).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.SimpleBooleanPropertyEqualsSwap), simpleConstant);
        }

        [Test]
        public async Task TwoBooleans()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == true).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.TwoBooleans), simpleConstant);
        }

        [Test]
        public async Task BooleanVar()
        {
            bool test = false;
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == test).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.BooleanVar), simpleConstant);
        }

        [Test]
        public async Task ComplexBoolean()
        {
            DataModel test = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => test.Boolean).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.ComplexBoolean), simpleConstant);
        }

        [Test]
        public async Task ComplexBooleanAsArgument()
        {
            DataModel test = new DataModel();
            await InternalComplexBooleanAsArgument(test);

        }

        private async Task InternalComplexBooleanAsArgument(DataModel test)
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => test.Boolean).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.ComplexBoolean), simpleConstant);
        }

        [Test]
        public async Task ThreeBooleans()
        {
            DataModel test = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean && r.Boolean && r.Boolean).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.ThreeBooleans), simpleConstant);
        }

        [Test]
        public async Task ListInt()
        {
            List<int> integers = new List<int>() { 1, 2, 3 };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => integers.Contains(r.Integer)).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.ListInt), simpleConstant);
        }


        [Test]
        public async Task ListIntFromProperty()
        {
            List<int> integers = new List<int>() { 1, 2, 3 };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.ListIntFromProperty), simpleConstant);
        }

        [Test]
        public async Task InnerLambda()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.ListInteger.Any(s => s == 1)).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.InnerLambda), simpleConstant);
        }

        [Test]
        public async Task NullableHasValue()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.OneToOneNullable.Integer.HasValue).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.NullableHasValue), simpleConstant);
        }

        [Test]
        public async Task NullableValue()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.NullableValue), simpleConstant);
        }

        [Test]
        public async Task LinqlObject()
        {
            LinqlObject<DataModel> objectTest = new LinqlObject<DataModel>(new DataModel());
            Assert.That(objectTest.TypedValue, Is.EqualTo(objectTest.Value));
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => objectTest.TypedValue.Integer == r.Integer).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.LinqlObject), simpleConstant);
        }

        [Test]
        public async Task LinqlObject_NonZero()
        {
            LinqlObject<DataModel> objectTest = new LinqlObject<DataModel>(new DataModel());
            objectTest.TypedValue.Integer = 1;
            Assert.That(objectTest.TypedValue, Is.EqualTo(objectTest.Value));
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => objectTest.TypedValue.Integer == r.Integer).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.LinqlObject_NonZero), simpleConstant);
        }

        [Test]
        public async Task ObjectCalculationWithNull()
        {
            DataModel objectTest = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => objectTest.OneToOne.Integer == r.Integer).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.ObjectCalculationWithNull), simpleConstant);
        }

        [Test]
        public async Task ObjectCalculationWithoutNull()
        {
            DataModel objectTest = new DataModel();
            objectTest.OneToOne = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => objectTest.OneToOne.Integer == r.Integer).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.ObjectCalculationWithoutNull), simpleConstant);
        }

        [Test]
        public async Task List_Int_Contains()
        {
            List<int> intList = new List<int>() { 1, 2, 3 };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string contains = await search.Where(r => intList.Contains(r.Integer)).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.List_Int_Contains), contains);
        }

        [Test]
        public async Task String_Contains()
        {
            
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string contains = await search.Where(r => "3".ToLowerInvariant().Contains(r.String)).ToJsonAsync();
            contains.ToLower();
            TestLoader.Compare(nameof(Smoke_Test.String_Contains), contains);
        }

        [Test]
        public async Task List_Int_Count()
        {
            List<int> intList = new List<int>() { 1, 2, 3 };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string contains = await search.Where(r => intList.Count == 1).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.List_Int_Count), contains);
        }

        [Test]
        public async Task Inner_Lambda()
        {
         
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string contains = await search.Where(r => r.ListRecusrive.Any(s => s.ListInteger.Contains(1))).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.Inner_Lambda), contains);
        }

        [Test]
        public async Task Select_Test()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string contains = await search.Select(r => r.Integer).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.Select_Test), contains);
        }

        [Test]
        public async Task SelectMany()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string contains = await search.SelectMany(r => r.ListInteger).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.SelectMany), contains);
        }

        [Test]
        public async Task SelectManyDouble()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string contains = await search.SelectMany(r => r.ListRecusrive.SelectMany(s => s.ListInteger)).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.SelectManyDouble), contains);
        }

        [Test]
        public async Task ToListAsync()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch jsonSearch = search.SelectMany(r => r.ListRecusrive.SelectMany(s => s.ListInteger)).ToListAsyncSearch();
            string value = JsonSerializer.Serialize(jsonSearch, new JsonSerializerOptions() { WriteIndented = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault });
            TestLoader.Compare(nameof(Smoke_Test.ToListAsync), value);
        }

        [Test]
        public async Task FirstOrDefault()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch jsonSearch = search.FirstOrDefaultAsyncSearch();
            string value = JsonSerializer.Serialize(jsonSearch, new JsonSerializerOptions() { WriteIndented = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault });
            TestLoader.Compare(nameof(Smoke_Test.FirstOrDefault), value);
        }

        [Test]
        public async Task FirstOrDefaultWithPredicate()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch jsonSearch = search.FirstOrDefaultAsyncSearch(r => r.Integer == 1);
            string value = JsonSerializer.Serialize(jsonSearch, new JsonSerializerOptions() { WriteIndented = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault });
            TestLoader.Compare(nameof(Smoke_Test.FirstOrDefaultWithPredicate), value);
        }


        [Test]
        public async Task LastOrDefault()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch jsonSearch = search.LastOrDefaultAsyncSearch();
            string value = JsonSerializer.Serialize(jsonSearch, new JsonSerializerOptions() { WriteIndented = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault });
            TestLoader.Compare(nameof(Smoke_Test.LastOrDefault), value);
        }

        [Test]
        public async Task LastOrDefaultWithPredicate()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch jsonSearch = search.LastOrDefaultAsyncSearch(r => r.Integer == 1);
            string value = JsonSerializer.Serialize(jsonSearch, new JsonSerializerOptions() { WriteIndented = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault });
            TestLoader.Compare(nameof(Smoke_Test.LastOrDefaultWithPredicate), value);
        }

        [Test]
        public async Task SkipTake()
        {

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch jsonSearch = search.Skip(1).Take(2).ToListAsyncSearch();
            string value = JsonSerializer.Serialize(jsonSearch, new JsonSerializerOptions() { WriteIndented = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault });
            TestLoader.Compare(nameof(Smoke_Test.SkipTake), value);
        }

        [Test]
        public async Task EmptyList()
        {

            List<int> intList = new List<int>() {  };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string contains = await search.Where(r => intList.Contains(r.Integer)).ToJsonAsync();
            TestLoader.Compare(nameof(Smoke_Test.EmptyList), contains);

        }
    }

}