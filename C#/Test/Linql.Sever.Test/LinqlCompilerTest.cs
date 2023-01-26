using Linql.Core;
using Linql.Test.Files;
using System.Text.Json;

namespace Linql.Server.Test
{
    public class LinqlCompilerTest : TestFileTests
    {
 
        [Test]
        public void Constructor()
        {
            //string json = this.TestLoader.TestFiles["Function"];
            //LinqlExpression? search = JsonSerializer.Deserialize<LinqlExpression>(json);

            string json = this.TestLoader.TestFiles.LastOrDefault().Value;
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() =>
            {
                if (search != null)
                {
                    LinqlCompiler compiler = new LinqlCompiler(search);
                }
            });
        }

        [Test]
        public void SearchIsSet()
        {
            string json = this.TestLoader.TestFiles.LastOrDefault().Value;
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() =>
            {
                if (search != null)
                {
                    DerivedCompiler compiler = new DerivedCompiler(search);
                    Assert.NotNull(compiler.GetSearch());
                }
            });
        }
    }

    internal class DerivedCompiler : LinqlCompiler
    {
        public LinqlSearch GetSearch()
        {
            return this.Search;
        }

        public DerivedCompiler(LinqlSearch Search) : base(Search)
        {
        }
    }
}