using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using NUnit.Framework;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{

    public static class AsyncExtensions
    {
        public static async Task<List<T>> ToListAsynct<T>(this IQueryable<T> query)
        {
            List<T> result = query.ToList();
            await Task.Delay(100);
            return result;
        }
    }

    internal class AsyncTest : TestFileTests
    {
        protected override string TestFolder { get; set; } = "SimpleExpressions";

        public IQueryable<DataModel> Data { get; set; }

        private LinqlCompiler Compiler = new LinqlCompiler(
            new HashSet<Assembly>()
            {
                typeof(System.Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly,
                typeof(AsyncExtensions).Assembly
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
        public void ToListAsync()
        {

            Assert.DoesNotThrow(() =>
            {
                LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
                LinqlSearch compiledSearch = search.Where(r => true).ToListAsyncSearch();

                List<DataModel> data = this.Compiler.Execute<List<DataModel>>(compiledSearch, this.Data);

                Assert.That(data.Count(), Is.EqualTo(this.Data.Count()));

            });

        }

       

    }

}
