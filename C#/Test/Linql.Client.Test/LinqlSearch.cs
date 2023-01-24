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

    }

    public class DerivedSearch<T> : LinqlSearch<T>
    {
      
    }

}