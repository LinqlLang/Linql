using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Server.EF6.Test.DataModel;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.EF6.Test
{
    public class BuildingTest : TestFileTests
    {

        EF6TestContext<Building> Context { get; set; } 
      
        [OneTimeSetUp]
        public override async Task Setup()
        {
            this.Context = new EF6TestContext<Building>();
            await this.Context.Init();
            //await base.Setup();
            //List<DataModel> dataList = new List<DataModel>();
            //dataList.Where(r => true).ToList();
            //foreach (int index in Enumerable.Range(1, 100))
            //{
            //    DataModel data = new CompiledDataModel(index, true);
            //    dataList.Add(data);
            //}

            //Data = dataList.AsQueryable();

            //HashSet<Assembly> assemblies = new HashSet<Assembly>()
            //{
            //    typeof(Boolean).Assembly,
            //    typeof(Enumerable).Assembly,
            //    typeof(Queryable).Assembly
            //};

            //HashSet<Assembly> extensionCompiler = new HashSet<Assembly>(assemblies);
            //extensionCompiler.Add(typeof(EF6Test).Assembly);

            //this.Compiler = new LinqlCompiler(assemblies);
            //this.ExtensionCompiler = new LinqlCompiler(extensionCompiler);


        }

        [Test]
        public void WhereFalse()
        {
        }

      

    }
}