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
            string json = this.TestLoader.TestFiles.FirstOrDefault().Value;
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            if (search != null) 
            {
                LinqlCompiler compiler = new LinqlCompiler(search);
                Assert.True(true);
            }
            else
            {
                Assert.Fail();
            }

        }
    }
}