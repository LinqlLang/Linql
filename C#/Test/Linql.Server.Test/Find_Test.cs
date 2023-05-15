using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{
    public class Find_Test
    {
        public IQueryable<DataModel> Data { get; set; }

        public LinqlCompiler Compiler { get; set; }


        [OneTimeSetUp]
        public async Task Setup()
        {
            List<DataModel> dataList = new List<DataModel>();
            dataList.Where(r => true).ToList();
            foreach (int index in Enumerable.Range(1, 100))
            {
                DataModel data = new CompiledDataModel(index, true);
                dataList.Add(data);
            }

            Data = dataList.AsQueryable();

            HashSet<Assembly> assemblies = new HashSet<Assembly>()
            {
                typeof(Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly
            };

            HashSet<Assembly> extensionCompiler = new HashSet<Assembly>(assemblies);
            extensionCompiler.Add(typeof(Application_Test).Assembly);

            this.Compiler = new LinqlCompiler(assemblies);


        }

        [Test]
        public void FindTrue()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => true);

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => true);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();

            List<LinqlExpression> findResults = baseCompiled.Find(compareCompiled);
            LinqlExpression firstResult = findResults.FirstOrDefault();
            Assert.IsTrue(findResults.Count == 1);
            Assert.IsTrue(firstResult.Equals(compareCompiled.Expressions[0].Next));
        }

        [Test]
        public void FindTrueShouldFail()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => true);

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => false);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();

            List<LinqlExpression> findResults = baseCompiled.Find(compareCompiled);
            Assert.IsTrue(findResults.Count == 0);
          
        }

        [Test]
        public void FindTrueDownChain()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => false).Where(r => true);

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => true);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();

            List<LinqlExpression> findResults = baseCompiled.Find(compareCompiled);
            LinqlExpression firstResult = findResults.FirstOrDefault();
            Assert.IsTrue(findResults.Count == 1);
            Assert.IsTrue(firstResult.Equals(compareCompiled.Expressions[0].Next));
        }


        [Test]
        public void FindTrueBothChains()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => true).Where(r => true);

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => true);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();

            List<LinqlExpression> findResults = baseCompiled.Find(compareCompiled);
            Assert.IsTrue(findResults.Count == 2);
        }

        [Test]
        public void FindTrueNextedChain()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.SelectMany(r => r.ListRecusrive.Where(s => true));

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => true);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();

            List<LinqlExpression> findResults = baseCompiled.Find(compareCompiled);
            Assert.IsTrue(findResults.Count == 1);
        }
    }
}