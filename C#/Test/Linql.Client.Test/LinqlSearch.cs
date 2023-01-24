using System.Reflection;

namespace Linql.Client.Test
{
    public class LinqlSearchTests : TestFileTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext();

        [Test]
        public void EmptySearch()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string empty = search.ToJson();

            this.TestLoader.Compare(nameof(LinqlSearchTests.EmptySearch), empty);
        }

        [Test]
        public async Task SimpleConstant()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => true).ToJsonAsync();
            this.TestLoader.Compare(nameof(LinqlSearchTests.SimpleConstant), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanProperty()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(LinqlSearchTests.SimpleBooleanProperty), simpleConstant);
        }

        [Test]
        public async Task SimpleBooleanPropertyChaining()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.OneToOne.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(LinqlSearchTests.SimpleBooleanPropertyChaining), simpleConstant);
        }
        
        [Test]
        public async Task SimpleBooleanPropertyEquals()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => r.Boolean == false).ToJsonAsync();
            this.TestLoader.Compare(nameof(LinqlSearchTests.SimpleBooleanPropertyEquals), simpleConstant);
        }

    }

    public class DerivedSearch<T> : LinqlSearch<T>
    {
      
    }

}