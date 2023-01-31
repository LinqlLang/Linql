using Linql.Core.Test;

namespace Linql.Client.Test
{
    public class LinqlSearch_Test
    {
        [OneTimeSetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor_Null_Provider()
        {
            Assert.Catch(() =>
            {
                LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(null);
            });

        }

        [Test]
        public void Constructor_Null_Expression()
        {
            Assert.Catch(() =>
            {
                LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(new LinqlContext(), null);
            });

        }


        [Test]
        public void ElementType()
        {

            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            Type elementType = search.ElementType;

            Assert.That(elementType, Is.EqualTo(typeof(DataModel)));

        }


        //[Test]
        //public void Constructor2()
        //{
        //    LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
        //    Assert.That(search.Type.TypeName, Is.EqualTo("DataModel"));
        //}

        ////[Test]
        ////public void GetEnumerator()
        ////{
        ////    LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();

        ////    try
        ////    {
        ////        foreach (DataModel item in search)
        ////        {

        ////        }
        ////    }
        ////    catch (EnumerationIsNotSupportedException ex)
        ////    {
        ////        Assert.IsTrue(true);
        ////        return;
        ////    }
        ////    Assert.IsTrue(false);
        ////}

        //[Test]
        //public void GetEnumerator2()
        //{
        //    LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();

        //    try
        //    {
        //        IEnumerable cast = (IEnumerable)search;
        //        cast.GetEnumerator();
        //    }
        //    catch (EnumerationIsNotSupportedException ex)
        //    {
        //        Assert.IsTrue(true);
        //        return;
        //    }
        //    Assert.IsTrue(false);
        //}

        //[Test]
        //public void ElementType()
        //{
        //    LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
        //    Assert.That(search.ElementType, Is.EqualTo(typeof(DataModel)));
        //}


        //[Test]
        //public void CustomProvider()
        //{
        //    CustomLinqlSearch<DataModel> search = new CustomLinqlSearch<DataModel>();
        //    Assert.That(search.Type.TypeName, Is.EqualTo("DataModel"));
        //}

        //[Test]
        //public async Task TestJsonOptions()
        //{
        //    List<int> integers = new List<int>() { 1, 2, 3 };
        //    CustomLinqlSearch<DataModel> search = new CustomLinqlSearch<DataModel>();

        //    string simpleConstant = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();

        //}

        //[Test]
        //public void NullProvider()
        //{
        //    try
        //    {
        //        LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsTrue(true);
        //        return;
        //    }
        //    Assert.IsTrue(false);

        //}

        //[Test]
        //public void NullExpression()
        //{
        //    try
        //    {
        //        LinqlSearch<DataModel> search = new LinqlSearch<DataModel>(new CustomLinqlProvider(), null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsTrue(true);
        //        return;
        //    }
        //    Assert.IsTrue(false);

        //}



    }



    //internal class CustomLinqlSearch<T> : LinqlSearch<T>
    //{
    //    public CustomLinqlSearch() : base(new CustomLinqlProvider()) { }
    //}
}