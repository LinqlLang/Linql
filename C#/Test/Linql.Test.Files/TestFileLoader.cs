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

        [OneTimeSetUp]
        public virtual async Task Setup()
        {
            TestLoader = new TestFileLoader(true);
            await TestLoader.LoadFiles();
        }

    }

    public class TestFileLoader
    {
        public Dictionary<string, string> TestFiles { get; set; } = new Dictionary<string, string>();

        protected bool WriteOutput { get; set; }

        public TestFileLoader(bool WriteOutput = false)
        {
            this.WriteOutput = WriteOutput;
        }

        public async Task LoadFiles()
        {
            List<string> files = Directory.GetFiles("./TestFiles", "*", searchOption: SearchOption.AllDirectories).ToList();

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
                string directory = "./Output";
                Directory.CreateDirectory(directory);
                File.WriteAllText($"{directory}/{TestName}.json", Output);
            }

            string testAgainst = TestFiles[TestName];
            Assert.That(Output, Is.EqualTo(testAgainst));
        }
    }
}
