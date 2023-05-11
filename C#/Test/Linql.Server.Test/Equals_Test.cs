using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{
    public class Equals_Test
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
        public void SimpleEquals()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => true);

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => true);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();

            Assert.IsTrue(baseCompiled.Equals(compareCompiled));
            
        }

        [Test]
        public void SimpleDoesNotEqual()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => true);

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => false);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();


            Assert.IsFalse(baseCompiled.Equals(compareCompiled));

        }

        [Test]
        public void SimpleDoesNotEqualExtendedOne()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => true).Where(r => false); 

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => true);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();


            Assert.IsFalse(baseCompiled.Equals(compareCompiled));

        }

        [Test]
        public void SimpleDoesNotEqualExtendedTwo()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => true);

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(r => true).Where(r => false);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();


            Assert.IsFalse(baseCompiled.Equals(compareCompiled));

        }

        [Test]
        public void SimpleEqualsParamChange()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();
            baseSearch = baseSearch.Where(r => true);

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();
            compare = compare.Where(s => true);

            LinqlSearch baseCompiled = baseSearch.ToLinqlSearch();
            LinqlSearch compareCompiled = compare.ToLinqlSearch();


            Assert.IsFalse(baseCompiled.Equals(compareCompiled));

        }

        [Test]
        public void SimplePropertyEquals()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();

            LinqlSearch baseCompiled = baseSearch.Select(r => r.Char).ToLinqlSearch();
            LinqlSearch compareCompiled = compare.Select(r => r.Char).ToLinqlSearch();


            Assert.IsTrue(baseCompiled.Equals(compareCompiled));

        }

        [Test]
        public void SimplePropertyDoesNotEquals()
        {
            IQueryable<DataModel> baseSearch = new LinqlSearch<DataModel>();

            IQueryable<DataModel> compare = new LinqlSearch<DataModel>();

            LinqlSearch baseCompiled = baseSearch.Select(r => r.Char).ToLinqlSearch();
            LinqlSearch compareCompiled = compare.Select(r => r.Byte).ToLinqlSearch();


            Assert.IsFalse(baseCompiled.Equals(compareCompiled));

        }


    }
}