using Linql.Client.Internal;
using Linql.Core.Test;
using Linql.Test.Files;

namespace Linql.Client.Test
{
    public class SmokeTest : TestFileTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext(null, new LinqlProviderPrettyPrint());

        protected override string TestFolder { get; set; } = "Smoke";

        [Test]
        public async Task MultipleClauses()
        {
            bool test = false;
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false).Where(r => true).ToJsonAsync();
            this.TestLoader.Compare(nameof(SmokeTest.MultipleClauses), simpleConstant);
        }

     
    }

}