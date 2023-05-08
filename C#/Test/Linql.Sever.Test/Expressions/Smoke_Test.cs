using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test.Expressions
{
    public class Smoke_Test : TestFileTests
    {

        protected override string TestFolder { get; set; } = "Smoke";


        public IQueryable<DataModel> Data { get; set; }

        private LinqlCompiler Compiler = new LinqlCompiler(
            new HashSet<Assembly>()
            {
                typeof(System.Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly,
                typeof(DataModel).Assembly
            },
            null,
            true
            );


        [OneTimeSetUp]
        public override async Task Setup()
        {
            await base.Setup();
            List<DataModel> dataList = new List<DataModel>();
            dataList.Where(r => true).ToList();
            foreach (int index in Enumerable.Range(1, 100))
            {
                DataModel data = new CompiledDataModel(index, true);
                data.Integer = index;
                dataList.Add(data);
            }

            Data = dataList.AsQueryable();

        }

        private void TestFile(string FileName, int Count) 
        {
            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles[FileName];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                IEnumerable<DataModel> data = this.Compiler.Execute<IEnumerable<DataModel>>(search, this.Data);

                Assert.That(data.Count(), Is.EqualTo(Count));

            });
        }


        [Test]
        public void EmptySearch()
        {
            this.TestFile(nameof(EmptySearch), 100);
        }

        [Test]
        public void TwoBooleans()
        {
            this.TestFile(nameof(TwoBooleans), 0);
        }

        [Test]
        public void ThreeBooleans()
        {
            this.TestFile(nameof(ThreeBooleans), 50);
        }

        [Test]
        public void BooleanNegate()
        {
            this.TestFile(nameof(BooleanNegate), 50);
        }

        [Test]
        public void BooleanVar()
        {
            this.TestFile(nameof(BooleanVar), 100);
        }

        [Test]
        public void ComplexBoolean()
        {
            this.TestFile(nameof(ComplexBoolean), 0);
        }

        [Test]
        public void LinqlObject()
        {
            this.TestFile(nameof(LinqlObject), 0);
        }

        [Test]
        public void LinqlObject_NonZero()
        {
            this.TestFile(nameof(LinqlObject_NonZero), 1);
        }

        [Test]
        public void List_Int_Contains()
        {
            this.TestFile(nameof(List_Int_Contains), 3);
        }

        //[Test]
        //public void List_Empty_Contains()
        //{
        //    this.TestFile(nameof(List_Empty_Contains), 0);
        //}



        [Test]
        public void SimpleConstant()
        {
            this.TestFile(nameof(SimpleConstant), 100);
        }

        [Test]
        public void InnerLambda()
        {
            this.TestFile(nameof(InnerLambda), 100);
        }

        [Test]
        public void List_Int_Count()
        {
            this.TestFile(nameof(List_Int_Count), 0);
        }

        [Test]
        public void ListInt()
        {
            this.TestFile(nameof(ListInt), 3);
        }

        [Test]
        public void ListIntFromProperty()
        {
            this.TestFile(nameof(ListIntFromProperty), 100);
        }

        [Test]
        public void NullableHasValue()
        {
            this.TestFile(nameof(NullableHasValue), 50);
        }

        [Test]
        public void NullableValue()
        {
            this.TestFile(nameof(NullableValue), 50);
        }

        [Test]
        public void ObjectCalculationWithNull()
        {
            this.TestFile(nameof(ObjectCalculationWithNull), 0);
        }

        [Test]
        public void ObjectCalculationWithoutNull()
        {
            this.TestFile(nameof(ObjectCalculationWithoutNull), 0);
        }


        [Test]
        public void SimpleBooleanFalse()
        {
            this.TestFile(nameof(SimpleBooleanFalse), 0);
        }

        [Test]
        public void SimpleBooleanProperty()
        {
            this.TestFile(nameof(SimpleBooleanProperty), 50);
        }

        [Test]
        public void SimpleBooleanPropertyChaining()
        {
            this.TestFile(nameof(SimpleBooleanPropertyChaining), 50);
        }

        [Test]
        public void SimpleBooleanPropertyEquals()
        {
            this.TestFile(nameof(SimpleBooleanPropertyEquals), 50);
        }

        [Test]
        public void SimpleBooleanPropertyEqualsSwap()
        {
            this.TestFile(nameof(SimpleBooleanPropertyEqualsSwap), 50);
        }

        [Test]
        public void String_Contains()
        {
            this.TestFile(nameof(String_Contains), 0);
        }

        [Test]
        public void Select_Test()
        {

            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles[nameof(Select_Test)];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                object data = this.Compiler.Execute(search, this.Data);

                Assert.True(data.GetType().IsEnumerable());

                IEnumerable<int> intData = this.Compiler.Execute<IEnumerable<int>>(search, this.Data);

                Assert.That(intData.FirstOrDefault(), Is.EqualTo(1));

            });
            
        }

        [Test]
        public void SelectMany()
        {

            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles[nameof(SelectMany)];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                //this.Data.SelectMany(r => r.ListInteger)

                object data = this.Compiler.Execute(search, this.Data);

                Assert.True(data.GetType().IsEnumerable());

                IEnumerable<int> intData = this.Compiler.Execute<IEnumerable<int>>(search, this.Data);

                Assert.That(intData.FirstOrDefault(), Is.EqualTo(1));

            });

        }


        [Test]
        public void SelectManyDouble()
        {

            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles[nameof(SelectManyDouble)];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                //this.Data.SelectMany(r => r.ListInteger)

                object data = this.Compiler.Execute(search, this.Data);

                Assert.True(data.GetType().IsEnumerable());

                IEnumerable<int> intData = this.Compiler.Execute<IEnumerable<int>>(search, this.Data);

                Assert.That(intData.FirstOrDefault(), Is.EqualTo(1));

            });

        }

        [Test]
        public void ToListAsync()
        {
            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles[nameof(ToListAsync)];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                IEnumerable<int> data = this.Compiler.Execute<IEnumerable<int>>(search, this.Data);

                Assert.That(data.Count(), Is.EqualTo(1000000));

            });
        }


        [Test]
        public void FirstOrDefault()
        {
            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles[nameof(FirstOrDefault)];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                DataModel data = this.Compiler.Execute<DataModel>(search, this.Data);

                Assert.That(data.Integer, Is.EqualTo(1));

            });
        }

        [Test]
        public void FirstOrDefaultWithPredicate()
        {
            Assert.DoesNotThrow(() =>
            {
                string json = this.TestLoader.TestFiles[nameof(FirstOrDefaultWithPredicate)];
                LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

                DataModel data = this.Compiler.Execute<DataModel>(search, this.Data);

                Assert.That(data.Integer, Is.EqualTo(1));

            });
        }

    }

}