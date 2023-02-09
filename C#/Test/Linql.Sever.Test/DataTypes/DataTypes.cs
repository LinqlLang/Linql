using Linql.Core;
using Linql.Core.Test;
using Linql.Server.Test;
using Linql.Server;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Client.Server.Expressions
{
    public partial class DataTypesTest : TestFileTests
    {

        protected override string TestFolder { get; set; } = "DataTypes";


        public IQueryable<DataModel> Data { get; set; }

        private LinqlCompiler Compiler = new LinqlCompiler(
            new HashSet<Assembly>()
            {
                typeof(System.Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly,
                typeof(DataModel).Assembly
            },
            null, 
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

                DataModel data = this.Compiler.Execute<DataModel>(search, this.Data);

                Assert.That(data, Is.Not.EqualTo(null));

            });
        }


        [Test]
        public void Guid()
        {
            this.TestFile(nameof(Guid), 1);
        }
    }

}