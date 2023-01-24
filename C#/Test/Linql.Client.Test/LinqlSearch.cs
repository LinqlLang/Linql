using System.Reflection;

namespace Linql.Client.Test
{
    public class LinqlSearchTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext();

        protected Dictionary<string, string> TestFiles { get; set; } = new Dictionary<string, string>();

        [SetUp]
        public async Task Setup()
        {
            List<string> files = Directory.GetFiles("./TestFiles").ToList();

            foreach(string file in files)
            {

                string FileName = Path.GetFileNameWithoutExtension(file);
                string text = await File.ReadAllTextAsync(file);
                this.TestFiles.Add(FileName, text);
            }
            
        }
       
        [Test]
        public void EmptySearch()
        {
            //DerivedSearch<DataModel> instanceMethod = new DerivedSearch<DataModel>();
            //string instanceMethodString = instanceMethod.NonExtensionMethod().ToJson();

            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string empty = search.ToJson();
            string testAgainst = this.TestFiles[nameof(EmptySearch)];
            //string test = search.Where(r => true == r.Boolean).ToJson();
            Assert.That(testAgainst, Is.EqualTo(empty));
        }

        [Test]
        public async Task SimpleConstant()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string json = await search.Where(r => r.OneToOne.Boolean).ToJsonAsync();
        }

    }

    public class DerivedSearch<T> : LinqlSearch<T>
    {
      
    }

}