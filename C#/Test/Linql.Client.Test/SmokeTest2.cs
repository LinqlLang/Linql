using Linql.Client.Internal;
using Linql.Core.Test;
using Linql.Test.Files;

namespace Linql.Client.Test
{
    public class SmokeTest2 : TestFileTests
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
            catch(UnsupportedIQueryableException ex)
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
            catch (UnsupportedIQueryableException ex)
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

            this.TestLoader.Compare(nameof(SmokeTest2.EmptySearch), empty);
        }

        [Test]
        public async Task SimpleConstant()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => true).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.SimpleConstant), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanProperty()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.SimpleBooleanProperty), simpleConstant);
        }

        [Test]
        public async Task BooleanNegate()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => !r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.BooleanNegate), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanPropertyChaining()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.OneToOne.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.SimpleBooleanPropertyChaining), simpleConstant);
        }
        
        [Test]
        public async Task SimpleBooleanPropertyEquals()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean == false).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.SimpleBooleanPropertyEquals), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanPropertyEqualsSwap()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.SimpleBooleanPropertyEqualsSwap), simpleConstant);
        }

        [Test]
        public async Task TwoBooleans()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == true).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.TwoBooleans), simpleConstant);
        }

        [Test]
        public async Task BooleanVar()
        {
            bool test = false;
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == test).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.BooleanVar), simpleConstant);
        }

        [Test]
        public async Task ComplexBoolean()
        {
            DataModel test = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => test.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.ComplexBoolean), simpleConstant);
        }

        [Test]
        public async Task ComplexBooleanAsArgument()
        {
            DataModel test = new DataModel();
            await this.InternalComplexBooleanAsArgument(test);
         
        }

        private async Task InternalComplexBooleanAsArgument(DataModel test)
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => test.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.ComplexBoolean), simpleConstant);
        }

        [Test]
        public async Task ThreeBooleans()
        {
            DataModel test = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean && r.Boolean && r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.ThreeBooleans), simpleConstant);
        }

        [Test]
        public async Task ListInt()
        {
            List<int> integers = new List<int>() { 1, 2, 3 };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => integers.Contains(r.Integer)).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.ListInt), simpleConstant);
        }


        [Test]
        public async Task ListIntFromProperty()
        {
            List<int> integers = new List<int>() { 1, 2, 3 };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.ListIntFromProperty), simpleConstant);
        }

        [Test]
        public async Task InnerLambda()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.ListInteger.Any(s => s ==1)).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.InnerLambda), simpleConstant);
        }

        [Test]
        public async Task NullableHasValue()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.OneToOneNullable.Integer.HasValue).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.NullableHasValue), simpleConstant);
        }

        [Test]
        public async Task NullableValue()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.NullableValue), simpleConstant);
        }

        [Test]
        public async Task LinqlObject()
        {
            LinqlObject<DataModel> objectTest = new LinqlObject<DataModel>(new DataModel());
            Assert.That(objectTest.TypedValue, Is.EqualTo(objectTest.Value));
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => objectTest.TypedValue.Integer == r.Integer).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.LinqlObject), simpleConstant);
        }

        [Test]
        public async Task ObjectCalculationWithNull()
        {
            DataModel objectTest = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => objectTest.OneToOne.Integer == r.Integer).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.ObjectCalculationWithNull), simpleConstant);
        }

        [Test]
        public async Task ObjectCalculationWithoutNull()
        {
            DataModel objectTest = new DataModel();
            objectTest.OneToOne = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => objectTest.OneToOne.Integer == r.Integer).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.ObjectCalculationWithoutNull), simpleConstant);
        }


        [Test]
        public async Task MultipleClauses()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => true).Where(r=> false).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest2.MultipleClauses), simpleConstant);
        }
    }

}