using Linql.Client;
using Linql.Core;
using Linql.Server.EF6.Test.DataModel;
using Linql.Test.Files;
using System.Data.Entity;
using System.Reflection;

namespace Linql.Server.EF6.Test
{
    public class BuildingTest : TestFileTests
    {
        bool ResetDatabase { get; set; } = false;
        EF6TestContext Context { get; set; }

        public LinqlCompiler Compiler { get; set; }

        [OneTimeSetUp]
        public override async Task Setup()
        {
            this.Context = new EF6TestContext();
            await this.Context.Init(ResetDatabase);

            HashSet<Assembly> assemblies = new HashSet<Assembly>()
            {
                typeof(Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly,
                typeof(DbContext).Assembly
            };


            this.Compiler = new LinqlCompiler(assemblies);
          
        }

        [Test]
        public void Take10()
        {
            Assert.DoesNotThrow(() =>
            {
                int take = 10;
                LinqlSearch<Building> search = new LinqlSearch<Building>();
                LinqlSearch compiledSearch = search.Take(take).ToListAsyncSearch();

                List<Building> data = this.Compiler.Execute<List<Building>>(compiledSearch, this.Context.Buildings);

                Assert.That(data.Count(), Is.EqualTo(take));

            });
        }

      

    }
}