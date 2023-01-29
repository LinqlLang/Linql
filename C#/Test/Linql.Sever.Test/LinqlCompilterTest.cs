using Linql.Client.Internal;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Linql.Server.Test
{
    internal class LinqlCompilterTest : LinqlCompiler
    {
        private TestFileLoader TestLoader = new TestFileLoader(true);

        [OneTimeSetUp]
        public async Task Setup()
        {

            this.ValidAssemblies = new HashSet<Assembly>()
            {
                typeof(System.Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly
            };

            this.UseCache = true;

            await this.TestLoader.LoadFiles();
        }

        [Test]
        public void FindWhereQueryable()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault() as LinqlFunction;

            MethodInfo methodToComapre = typeof(Queryable).GetMethods().First(r => r.Name == "Where");

            MethodInfo foundMethod = this.FindMethod(typeof(IQueryable<DataModel>), function);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));
        }

        [Test]
        public void FindWhereEnumerable()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault() as LinqlFunction;

            MethodInfo methodToComapre = typeof(Enumerable).GetMethods().First(r => r.Name == "Where");

            MethodInfo foundMethod = this.FindMethod(typeof(IEnumerable<DataModel>), function);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));
        }

        [Test]
        //Since cache is turned on at the start, and I've queried using Queryable before, the methods will be loaded. 
        //Therefore, even though I've removed Queryable from the searchable assemblies, the cache should pick up the queryable method
        //Then, I turn caching off, and ensure that I don't get the queryable method back. 
        public void TestMethodCaching()
        {
            Type enumerableType = typeof(IEnumerable<DataModel>);
            Type queryableType = typeof(IQueryable<DataModel>);

            MethodInfo enumerableWhere = typeof(Enumerable).GetMethods().First(r => r.Name == "Where");
            MethodInfo queryableWhere = typeof(Queryable).GetMethods().First(r => r.Name == "Where");


            this.ClearMethodCache();

            this.ValidAssemblies.Remove(typeof(Queryable).Assembly);
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            
            
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault() as LinqlFunction;


            MethodInfo foundMethod = this.FindMethod(queryableType, function);

            Assert.That(foundMethod, Is.EqualTo(enumerableWhere));

            this.ValidAssemblies.Add(typeof(Queryable).Assembly);


            foundMethod = this.FindMethod(queryableType, function);

            Assert.That(foundMethod, Is.EqualTo(enumerableWhere));

            this.UseCache = false;

            foundMethod = this.FindMethod(queryableType, function);

            Assert.That(foundMethod, Is.EqualTo(queryableWhere));

            this.UseCache = true;
            this.ClearMethodCacheForType(queryableType);

            foundMethod = this.FindMethod(queryableType, function);

            Assert.That(foundMethod, Is.EqualTo(queryableWhere));
        }

        [Test]
        public void FindSelectIQueryable()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault() as LinqlFunction;

            function.FunctionName = "Select";

            MethodInfo methodToComapre = typeof(Queryable).GetMethods().First(r => r.Name == "Select");

            MethodInfo foundMethod = this.FindMethod(typeof(IQueryable<DataModel>), function);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));
        }

        [Test]
        public void FindSelectEnumerable()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault() as LinqlFunction;

            function.FunctionName = "Select";

            MethodInfo methodToComapre = typeof(Enumerable).GetMethods().First(r => r.Name == "Select");

            MethodInfo foundMethod = this.FindMethod(typeof(IEnumerable<DataModel>), function);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));
        }

        [Test]
        public void ExecuteShouldErrorIfNotAFunction()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            search.Expressions[0] = new LinqlConstant();

            Assert.Catch(() => this.Execute(search, new List<DataModel>()));

        }

        [Test]
        public void ShouldErrorIfMethodNotFound()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault() as LinqlFunction;
            function.FunctionName = "DummyMethod";

            Assert.Catch(() => this.FindMethod(typeof(IEnumerable<DataModel>), function));

        }


    }

}
