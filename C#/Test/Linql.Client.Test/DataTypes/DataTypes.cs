using Linql.Core.Test;
using Linql.Test.Files;

namespace Linql.Client.Test.Expressions
{
    public partial class DataTypesTest : TestFileTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext();

        protected override string TestFolder { get; set; } = "DataTypes";

        protected JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
        };


        [Test]
        public void DateTime_Equals()
        {
            DateTime now = DateTime.Now;
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch compiledSearch = search.FirstOrDefaultAsyncSearch(r => r.DateTime == now);
            string value = JsonSerializer.Serialize(compiledSearch, JsonSerializerOptions);

            compiledSearch = search.FirstOrDefaultAsyncSearch(r => r.DateTime == DateTime.Now);
            string value2 = JsonSerializer.Serialize(compiledSearch, JsonSerializerOptions);

            //TestLoader.Compare(nameof(DateTime_Equals), value);
        }

        [Test]
        public void Guid()
        {
            Guid guid = DataModel.GuidAnchor;
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch compiledSearch = search.FirstOrDefaultAsyncSearch(r => r.Guid == guid);
            string value = JsonSerializer.Serialize(compiledSearch, JsonSerializerOptions);

            TestLoader.Compare(nameof(Guid), value);
        }

    }

}