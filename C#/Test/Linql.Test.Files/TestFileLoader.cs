using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Test.Files
{
    public abstract class TestFileTests
    {
        public TestFileLoader TestLoader { get; set; }

        protected virtual string TestFolder { get; set; }

        [OneTimeSetUp]
        public virtual async Task Setup()
        {
            TestLoader = new TestFileLoader(this.TestFolder, true);
            await TestLoader.LoadFiles();
        }

    }

    public class TestFileLoader
    {
        public Dictionary<string, string> TestFiles { get; set; } = new Dictionary<string, string>();

        protected bool WriteOutput { get; set; }

        protected string BasePath { get; set; }

        public TestFileLoader(string BasePath = "", bool WriteOutput = false)
        {
            this.WriteOutput = WriteOutput;
            this.BasePath = BasePath;
        }

        public async Task LoadFiles()
        {
            List<string> files = Directory.GetFiles(Path.Combine("./TestFiles", this.BasePath), "*", searchOption: SearchOption.AllDirectories).ToList();

            foreach (string file in files)
            {

                string FileName = Path.GetFileNameWithoutExtension(file);
                string text = File.ReadAllText(file);
                TestFiles.Add(FileName, text);
            }
        }

        public void Compare(string TestName, string Output)
        {
            if (WriteOutput)
            {
                string directory = Path.Combine("./Output", this.BasePath);
                Directory.CreateDirectory(directory);
                File.WriteAllText($"{directory}/{TestName}.json", Output);
            }

            string testAgainst = TestFiles[TestName];
            Assert.That(Output, Is.EqualTo(testAgainst));
        }
    }
}
