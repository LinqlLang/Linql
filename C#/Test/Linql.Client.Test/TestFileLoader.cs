using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Client.Test
{
    public abstract class TestFileTests
    {
        public TestFileLoader TestLoader { get; set; }

        [SetUp]
        public async Task Setup()
        {
            this.TestLoader = new TestFileLoader();
            await this.TestLoader.LoadFiles();
        }

    }

    public class TestFileLoader
    {
        protected Dictionary<string, string> TestFiles { get; set; } = new Dictionary<string, string>();

        public async Task LoadFiles()
        {
            List<string> files = Directory.GetFiles("./TestFiles").ToList();

            foreach (string file in files)
            {

                string FileName = Path.GetFileNameWithoutExtension(file);
                string text = await File.ReadAllTextAsync(file);
                this.TestFiles.Add(FileName, text);
            }
        }

        public void Compare(string TestName, string Output)
        {
            string testAgainst = this.TestFiles[TestName];
            Assert.That(Output, Is.EqualTo(testAgainst));
        }
    }
}
