using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{
    public class Application_Test : TestFileTests
    {
        public IQueryable<DataModel> Data { get; set; }

        public LinqlCompiler Compiler { get; set; }

        public LinqlCompiler ExtensionCompiler { get; set; }


        protected override string TestFolder { get; set; } = "Smoke";

        [OneTimeSetUp]
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

            HashSet<Assembly> assemblies = new HashSet<Assembly>()
            {
                typeof(Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly
            };

            HashSet<Assembly> extensionCompiler = new HashSet<Assembly>(assemblies);
            extensionCompiler.Add(typeof(Application_Test).Assembly);

            this.Compiler = new LinqlCompiler(assemblies);
            this.ExtensionCompiler = new LinqlCompiler(extensionCompiler);


        }

        [Test]
        public void WhereFalse()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> data = this.Compiler.Execute<IEnumerable<DataModel>>(search, this.Data);

                Assert.That(data.Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public void WhereFalseQueryable()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() =>
            {
                IQueryable<DataModel> data = this.Compiler.Execute<IQueryable<DataModel>>(search, this.Data);

                Assert.That(data.Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public async Task WhereFalseLinqlSearch()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            LinqlSearch<DataModel> data = new LinqlSearch<DataModel>();

            IQueryable<DataModel> result = this.Compiler.Execute<IQueryable<DataModel>>(search, data);
            string test = await result.ToJsonAsync();
            this.TestLoader.Compare("SimpleBooleanFalse", test);
        }

        [Test]
        public async Task SimpleBooleanPropertyInception()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanProperty"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            LinqlSearch<DataModel> data = new LinqlSearch<DataModel>();

            IQueryable<DataModel> result = this.Compiler.Execute<IQueryable<DataModel>>(search, data);
            string test = await result.ToJsonAsync();
            this.TestLoader.Compare("SimpleBooleanProperty", test);
        }

        [Test]
        public async Task SimpleBooleanPropertyData()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanProperty"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            IQueryable<DataModel> data = this.Data;

            IQueryable<DataModel> result = this.Compiler.Execute<IQueryable<DataModel>>(search, data);
            List<DataModel> compiledResult = result.ToList();
            Assert.That(result.Count(), Is.EqualTo(this.Data.Count() / 2));

        }

        [Test]
        public async Task MultipleClauses()
        {
            string json = this.TestLoader.TestFiles["BooleanVar"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            IQueryable<DataModel> data = this.Data;

            IQueryable<DataModel> result = this.Compiler.Execute<IQueryable<DataModel>>(search, data);
            List<DataModel> compiledResult = result.ToList();
            Assert.That(result.Count(), Is.EqualTo(100));

        }

        [Test]
        public async Task SkipTake()
        {
            string json = this.TestLoader.TestFiles["SkipTake"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            IQueryable<DataModel> data = this.Data;

            List<DataModel> result = this.Compiler.Execute<List<DataModel>>(search, data);
            List<DataModel> compiledResult = result.ToList();
        }


        [Test]
        public void LinqlObjectNotLoaded()
        {
            this.Compiler.ValidAssemblies.Remove(typeof(DataModel).Assembly);

            string json = this.TestLoader.TestFiles["LinqlObject"];
            IQueryable<DataModel> data = this.Data;
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.Catch(() => this.Compiler.Execute<IQueryable<DataModel>>(search, data));


        }


        [Test]
        public void LinqlObject()
        {
            this.Compiler.ValidAssemblies.Add(typeof(DataModel).Assembly);
            string json = this.TestLoader.TestFiles["LinqlObject"];
            IQueryable<DataModel> data = this.Data;
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() => this.Compiler.Execute<IQueryable<DataModel>>(search, data));


        }


        [Test]
        public void ListWithLInqlObject()
        {

            this.Data.FirstOrDefault(r => r.Boolean);
            this.Compiler.ValidAssemblies.Add(typeof(DataModel).Assembly);
            string json = this.TestLoader.TestFiles["LinqlObject"];
            List<DataModel> data = this.Data.ToList();
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() => this.Compiler.Execute<IEnumerable<DataModel>>(search, data));


        }


        [Test]
        public void UserSuppliedListContains()
        {
            List<string> names = new List<string>() { "test" };
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => names.Select(s => s.ToUpper()).Contains(r.String)).ToLinqlSearch();
            Assert.DoesNotThrow(() => this.Compiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable()));


        }


        [Test]
        public void DateTimeProperties()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => r.DateTime.Year == DateTime.Now.Year).ToLinqlSearch();
            Assert.DoesNotThrow(() => this.Compiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable()));


        }

        [Test]
        public void TestExtensionMethodShouldFail()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => r.TestQueryableExtensionMethod()).ToLinqlSearch();
            Assert.Catch(() => this.Compiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable()));

        }

        [Test]
        public void TestExtensionMethod()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => r.TestQueryableExtensionMethod()).ToLinqlSearch();
            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> data = this.ExtensionCompiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable());
                Assert.That(data.Count(), Is.EqualTo(this.Data.Count()));

            });

        }

        [Test]
        public void TestExtensionMethodGeneric()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => r.TestQueryableExtensionMethod()).ToLinqlSearch();
            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> data = this.ExtensionCompiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable());
                Assert.That(data.Count(), Is.EqualTo(this.Data.Count()));

            });

        }

        [Test]
        public void TestExtensionMethodGenericInterface()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => r.TestQueryableGenericExtensionMethodInterface()).ToLinqlSearch();
            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> data = this.ExtensionCompiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable());
                Assert.That(data.Count(), Is.EqualTo(this.Data.Count()));

            });

        }

        [Test]
        public void TestExtensionMethodGenericAbstract()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => r.TestQueryableGenericExtensionMethodAbstractClass()).ToLinqlSearch();
            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> data = this.ExtensionCompiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable());
                Assert.That(data.Count(), Is.EqualTo(this.Data.Count()));

            });

        }


        [Test]
        public void TestInterfaceExtensionMethod()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => r.TestInterfaceExtensionMethod()).ToLinqlSearch();
            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> data = this.ExtensionCompiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable());
                Assert.That(data.Count(), Is.EqualTo(this.Data.Count()));

            });

        }

        [Test]
        public void TestAbstractExtensionMethod()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            LinqlSearch compiledSearch = search.Where(r => r.TestAbstractExtensionMethod()).ToLinqlSearch();
            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> data = this.ExtensionCompiler.Execute<IEnumerable<DataModel>>(compiledSearch, this.Data.AsQueryable());
                Assert.That(data.Count(), Is.EqualTo(this.Data.Count()));

            });

        }


    }

    public static class ExtensionMethodTest
    {
        public static bool TestQueryableExtensionMethod(this DataModel source)
        {
            return true;
        }

        public static bool TestQueryableGenericExtensionMethod<T>(this T source) where T: DataModel
        {
            return true;
        }

        public static bool TestQueryableGenericExtensionMethodInterface<T>(this T source) where T : IDataModel
        {
            return true;
        }

        public static bool TestQueryableGenericExtensionMethodAbstractClass<T>(this T source) where T : ADataModel
        {
            return true;
        }

        public static bool TestInterfaceExtensionMethod(this IDataModel source)
        {
            return true;
        }

        public static bool TestAbstractExtensionMethod(this ADataModel source)
        {
            return true;
        }
    }


}