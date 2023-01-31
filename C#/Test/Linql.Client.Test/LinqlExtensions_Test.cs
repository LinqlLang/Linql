using Linql.Core.Test;
using RichardSzalay.MockHttp;
using System.Collections;
using System.Text;

namespace Linql.Client.Test
{
    public class LinqlExtensions_Test
    {
        [OneTimeSetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToJson_LinqlSearch()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();

            Assert.DoesNotThrow(() =>
            {
                search.Where(r => true).ToJson();
            });
        }

        [Test]
        public void ToJsonAsync_LinqlSearch()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();

            Assert.DoesNotThrowAsync(async () =>
            {
                await search.Where(r => true).ToJsonAsync();
            });
        }

        [Test]
        public void ToJson_IQueryable()
        {
            IQueryable<DataModel> search = (new List<DataModel>()).AsQueryable();

            Assert.Catch(() =>
            {
                search.Where(r => true).ToJson();
            });
        }

        [Test]
        public void ToJsonAsync_IQueryable()
        {
            IQueryable<DataModel> search = (new List<DataModel>()).AsQueryable();

            Assert.CatchAsync(async () =>
            {
                await search.Where(r => true).ToJsonAsync();
            });
        }


        [Test]
        public void ToListAsync_IQueryable()
        {
            IQueryable<DataModel> search = (new List<DataModel>()).AsQueryable();

            Assert.CatchAsync(async () =>
            {
                await search.Where(r => true).ToListAsync();
            });
        }

        [Test]
        public void ToLinqlSearch_IQueryable()
        {
            IQueryable<DataModel> search = (new List<DataModel>()).AsQueryable();

            Assert.DoesNotThrow(() =>
            {
                LinqlSearch test = search.Where(r => true).ToLinqlSearch();
            });
        }

    }
}