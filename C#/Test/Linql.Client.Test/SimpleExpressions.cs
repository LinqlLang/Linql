using Linql.Client.Internal;
using Linql.Core.Test;
using Linql.Test.Files;

namespace Linql.Client.Test
{
    public class SimpleExpressions : TestFileTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext();

        [Test]
        public async Task LinqlConstant()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string simpleConstant = await search.Where(r => false).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlConstant), simpleConstant);
        }

      
    }

}