using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{
    internal class SimpleExpressions_Test : TestFileTests
    {
        protected override string TestFolder { get; set; } = "SimpleExpressions";

        public IQueryable<DataModel> Data { get; set; }

        private LinqlCompiler Compiler = new LinqlCompiler(
            new HashSet<Assembly>()
            {
                typeof(System.Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly
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
                dataList.Add(data);
            }

            Data = dataList.AsQueryable();

        }


        [Test]
        public void FunctionChaining()
        {

            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles["FunctionChaining"];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                IQueryable<DataModel> data = this.Compiler.Execute<IQueryable<DataModel>>(search, this.Data);

                Assert.That(data.Count(), Is.EqualTo(0));

            });

        }

        [Test]
        public void ToList()
        {

            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles["ToList"];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                IEnumerable<DataModel> data = this.Compiler.Execute<IEnumerable<DataModel>>(search, this.Data);

                Assert.That(data.Count(), Is.EqualTo(50));

            });

        }

    }

}
