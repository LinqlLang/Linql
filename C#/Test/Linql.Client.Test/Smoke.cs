using System.Reflection;

namespace Linql.Client.Test
{
    public class Smoke : TestFileTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext();

        [Test]
        public void EmptySearch()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string empty = search.ToJson();

            this.TestLoader.Compare(nameof(Smoke.EmptySearch), empty);
        }

        [Test]
        public async Task SimpleConstant()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => true).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.SimpleConstant), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanProperty()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.SimpleBooleanProperty), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanPropertyChaining()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.OneToOne.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.SimpleBooleanPropertyChaining), simpleConstant);
        }
        
        [Test]
        public async Task SimpleBooleanPropertyEquals()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean == false).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.SimpleBooleanPropertyEquals), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanPropertyEqualsSwap()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.SimpleBooleanPropertyEqualsSwap), simpleConstant);
        }

        [Test]
        public async Task TwoBooleans()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == true).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.TwoBooleans), simpleConstant);
        }

        [Test]
        public async Task BooleanVar()
        {
            bool test = false;
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false == test).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.BooleanVar), simpleConstant);
        }

        [Test]
        public async Task ComplexBoolean()
        {
            DataModel test = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => test.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.ComplexBoolean), simpleConstant);
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
            this.TestLoader.Compare(nameof(Smoke.ComplexBoolean), simpleConstant);
        }

        [Test]
        public async Task ThreeBooleans()
        {
            DataModel test = new DataModel();
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean && r.Boolean && r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.ThreeBooleans), simpleConstant);
        }

        [Test]
        public async Task ListInt()
        {
            List<int> integers = new List<int>() { 1, 2, 3 };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => integers.Contains(r.Integer)).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.ListInt), simpleConstant);
        }


        [Test]
        public async Task ListIntFromProperty()
        {
            List<int> integers = new List<int>() { 1, 2, 3 };
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
            this.TestLoader.Compare(nameof(Smoke.ListIntFromProperty), simpleConstant);
        }


    }

    public class DerivedSearch<T> : LinqlSearch<T>
    {
      
    }

}