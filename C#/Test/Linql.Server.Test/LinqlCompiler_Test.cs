using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{
    internal class LinqlCompiler_Test : LinqlCompiler
    {
        private TestFileLoader TestLoader = new TestFileLoader("Smoke", true);


        [OneTimeSetUp]
        public async Task Setup()
        {

            this.ValidAssemblies = new HashSet<Assembly>()
            {
                typeof(System.Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(List<>).Assembly,
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
            LinqlFunction function = search.Expressions.FirstOrDefault().Next as LinqlFunction;

            MethodInfo methodToComapre = typeof(Queryable).GetMethods().First(r => r.Name == "Where");

            Type[] functionArgs = new Type[] { typeof(IQueryable<DataModel>), typeof(Func<DataModel, bool>) };

            MethodInfo foundMethod = this.FindMethod(typeof(IQueryable<DataModel>), function, functionArgs);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));

        }

        [Test]
        public void Find_Where_EnumerableQuery()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault().Next as LinqlFunction;

            MethodInfo methodToComapre = typeof(Queryable).GetMethods().First(r => r.Name == "Where");
            Type[] functionArgs = new Type[] { typeof(EnumerableQuery<DataModel>), typeof(Func<DataModel, bool>) };


            MethodInfo foundMethod = this.FindMethod(typeof(EnumerableQuery<DataModel>), function, functionArgs);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));
        }

        [Test]
        public void Find_Where_List()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault().Next as LinqlFunction;

            MethodInfo methodToComapre = typeof(Enumerable).GetMethods().First(r => r.Name == "Where");
            Type[] functionArgs = new Type[] { typeof(List<DataModel>), typeof(Func<DataModel, bool>) };

            MethodInfo foundMethod = this.FindMethod(typeof(List<DataModel>), function, functionArgs);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));
        }

        [Test]
        public void FindWhereEnumerable()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault().Next as LinqlFunction;

            MethodInfo methodToComapre = typeof(Enumerable).GetMethods().First(r => r.Name == "Where");

            Type[] functionArgs = new Type[] { typeof(IEnumerable<DataModel>), typeof(Func<DataModel, bool>) };

            MethodInfo foundMethod = this.FindMethod(typeof(List<DataModel>), function, functionArgs);

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
            LinqlFunction function = search.Expressions.FirstOrDefault().Next as LinqlFunction;
            Type[] functionArgs = new Type[] { typeof(IEnumerable<DataModel>), typeof(Func<DataModel, bool>) };


            MethodInfo foundMethod = this.FindMethod(queryableType, function, functionArgs);

            Assert.That(foundMethod, Is.EqualTo(enumerableWhere));

            this.ValidAssemblies.Add(typeof(Queryable).Assembly);


            foundMethod = this.FindMethod(queryableType, function, functionArgs);

            Assert.That(foundMethod, Is.EqualTo(enumerableWhere));

            this.UseCache = false;

            foundMethod = this.FindMethod(queryableType, function, functionArgs);

            Assert.That(foundMethod, Is.EqualTo(queryableWhere));

            this.UseCache = true;
            this.ClearMethodCacheForType(queryableType);

            foundMethod = this.FindMethod(queryableType, function, functionArgs);

            Assert.That(foundMethod, Is.EqualTo(queryableWhere));
        }

        [Test]
        public void FindSelectIQueryable()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault().Next as LinqlFunction;

            function.FunctionName = "Select";

            MethodInfo methodToComapre = typeof(Queryable).GetMethods().First(r => r.Name == "Select");
            Type[] functionArgs = new Type[] { typeof(IQueryable<DataModel>), typeof(Func<,>) };


            MethodInfo foundMethod = this.FindMethod(typeof(IQueryable<DataModel>), function, functionArgs);

            Assert.That(foundMethod, Is.EqualTo(methodToComapre));
        }

        [Test]
        public void FindSelectEnumerable()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);
            LinqlFunction function = search.Expressions.FirstOrDefault().Next as LinqlFunction;

            function.FunctionName = "Select";

            MethodInfo methodToComapre = typeof(Enumerable).GetMethods().First(r => r.Name == "Select");
            Type[] functionArgs = new Type[] { typeof(IQueryable<DataModel>), typeof(Func<,>) };

            MethodInfo foundMethod = this.FindMethod(typeof(IEnumerable<DataModel>), function, functionArgs);

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
            LinqlFunction function = search.Expressions.FirstOrDefault().Next as LinqlFunction;
            function.FunctionName = "DummyMethod";

            Type[] functionArgs = new Type[] { };

            Assert.Catch(() => this.FindMethod(typeof(IEnumerable<DataModel>), function, functionArgs));

        }


        [Test]
        public void Invalid_LinqlExpresion_Derivation()
        {
            Assert.Catch(() => this.Visit(new FakeLinqlExpression(), null, null));
        }

        [Test]
        public void Invalid_LinqlLambda_Null_InputType()
        {
            Assert.Catch(() => this.VisitLambda(null, null, null));
        }

        [Test]
        public void Invalid_LinqlConstant()
        {
            List<int> list = new List<int>();
            LinqlConstant constant = new LinqlConstant(typeof(List<int>).ToLinqlType(), list);
            string serialized = JsonSerializer.Serialize<LinqlExpression>(constant);

            LinqlExpression converted = JsonSerializer.Deserialize<LinqlExpression>(serialized);

            if (converted is LinqlConstant linqlConstant)
            {
                Assert.DoesNotThrow(() => this.VisitConstant(linqlConstant, null));
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void Invalid_LinqlConstant_Type()
        {
            List<int> list = new List<int>();
            LinqlConstant constant = new LinqlConstant(typeof(List<int>).ToLinqlType(), list);
            constant.ConstantType.TypeName = "dummy type";
            Assert.Catch(() => this.VisitConstant(constant, null));
        }


        [Test]
        public void Invalid_LinqlProperty()
        {
            LinqlProperty property = new LinqlProperty();
            Assert.Catch(() => this.VisitProperty(property, null));
        }


        [Test]
        public void Invalid_LinqlObject_Type()
        {
            LinqlObject obj = new LinqlObject();
            obj.Value = "test";
            obj.Type = new LinqlType();
            obj.Type.TypeName = "DummyType";
            Assert.Catch(() => this.VisitObject(obj, null));
        }


    }

    internal class FakeLinqlExpression : LinqlExpression
    {

    }

}
