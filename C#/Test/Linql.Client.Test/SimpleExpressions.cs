using Linql.Client.Internal;
using Linql.Core.Test;
using Linql.Test.Files;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Text.Json;

namespace Linql.Client.Test
{
    public class SimpleExpressions : TestFileTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext(null, new LinqlProviderPrettyPrint());

        protected override string TestFolder { get; set; } = "./SimpleExpressions";

        [Test]
        public async Task LinqlConstant()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => false).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlConstant), output);
        }

        [Test]
        public async Task LinqlBinary()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => r.Boolean && r.OneToOne.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlBinary), output);
        }

        [Test]
        public async Task LinqlUnary()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => !r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlUnary), output);
        }

        [Test]
        public async Task LinqlObject()
        {
            LinqlObject<DataModel> objectTest = new LinqlObject<DataModel>(new DataModel());
            Assert.That(objectTest.TypedValue, Is.EqualTo(objectTest.Value));
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => objectTest.TypedValue.Integer == r.Integer).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlObject), output);

        }

        [Test]
        public async Task LinqlFunction()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions.LinqlFunction), output);

        }


    }
}