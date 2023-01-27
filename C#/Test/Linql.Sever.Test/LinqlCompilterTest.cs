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

            this.ValidAssemblies = new List<Assembly>()
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
        public void DefaultWhereEnumerable()
        {
            this.ValidAssemblies.Remove(typeof(Queryable).Assembly);
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault() as LinqlFunction;

            MethodInfo methodToComapre = typeof(Enumerable).GetMethods().First(r => r.Name == "Where");

            MethodInfo foundMethod = this.FindMethod(typeof(IQueryable<DataModel>), function);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));

            this.ValidAssemblies.Add(typeof(Queryable).Assembly);

            methodToComapre = typeof(Queryable).GetMethods().First(r => r.Name == "Where");

            foundMethod = this.FindMethod(typeof(IQueryable<DataModel>), function);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));
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

    }

}
