using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{
    public class LinqlCompilerTest : TestFileTests
    {
        public IQueryable<DataModel> Data { get; set; }

        [SetUp]
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

        //[Test]
        //public void Constructor()
        //{
        //    //string json = this.TestLoader.TestFiles["Function"];
        //    //LinqlExpression? search = JsonSerializer.Deserialize<LinqlExpression>(json);

        //    string json = this.TestLoader.TestFiles.LastOrDefault().Value;
        //    LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

        //    Assert.DoesNotThrow(() =>
        //    {
        //        if (search != null)
        //        {
        //            LinqlCompiler compiler = new LinqlCompiler(search);
        //        }
        //    });
        //}

        //[Test]
        //public void SearchIsSet()
        //{
        //    string json = this.TestLoader.TestFiles.LastOrDefault().Value;
        //    LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

        //    Assert.DoesNotThrow(() =>
        //    {
        //        if (search != null)
        //        {
        //            DerivedCompiler compiler = new DerivedCompiler(search);
        //            Assert.NotNull(compiler.GetSearch());
        //        }
        //    });
        //}

        [Test]
        public void Execute()
        {
            //string json = this.TestLoader.TestFiles["Function"];
            //LinqlExpression? search = JsonSerializer.Deserialize<LinqlExpression>(json);
           
            List<Assembly> assemblies = new List<Assembly>() { typeof(Boolean).Assembly, typeof(Enumerable).Assembly };
            LinqlCompiler compiler = new LinqlCompiler(assemblies);
            string json = this.TestLoader.TestFiles["SimpleConstant"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() =>
            {
                compiler.Execute(search, this.Data);
            });
        }
    }

    //internal class DerivedCompiler : LinqlCompiler
    //{
    //    public LinqlSearch GetSearch()
    //    {
    //        return this.Search;
    //    }

    //    public DerivedCompiler(LinqlSearch Search) : base(Search)
    //    {
    //    }
    //}
}