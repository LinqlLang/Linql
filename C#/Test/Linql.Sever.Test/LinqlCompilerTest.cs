using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Text.Json;

namespace Linql.Server.Test
{
    public class LinqlCompilerTest : TestFileTests
    {
        public List<IQueryable<DataModel>> Data { get; set; } = new List<IQueryable<DataModel>>();

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();

            foreach (int index in Enumerable.Range(1, 100))
            {
                DataModel data = new DataModel();
            }

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
        public void Constructor()
        {
            //string json = this.TestLoader.TestFiles["Function"];
            //LinqlExpression? search = JsonSerializer.Deserialize<LinqlExpression>(json);

            string json = this.TestLoader.TestFiles.LastOrDefault().Value;
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() =>
            {
                search.Execute(this.Data.AsQueryable());
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