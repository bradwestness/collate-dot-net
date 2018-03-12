using Collate;
using Collate.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using Tests.Data;

namespace Tests
{
    [TestClass]
    public class SortTests
    {
        [TestMethod]
        public void SingleSortTest()
        {
            var request = new SortRequest
            {
                Sorts = new ISort[]
                {
                    new Sort
                    {
                        Field = nameof(Track.Name),
                        Direction = SortDirection.Ascending
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Sort(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var sorted = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(items.Count, sorted.Count);
                Assert.AreNotEqual(items[0].Name, sorted[0].Name);
                Assert.AreEqual("'Round Midnight", sorted[0].Name);
            }
        }

        [TestMethod]
        public void NavigationSortTest()
        {
            var navigationSort = new Sort
            {
                Field = nameof(Genre.Name),
                Direction = SortDirection.Ascending
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.NavigationSort(navigationSort, nameof(Track.Genre));
                var sql = ((DbQuery<Track>)queryable).Sql;
                var sorted = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(items.Count, sorted.Count);
                Assert.AreNotEqual(items[0].Genre.Name, sorted[0].Genre.Name);
                Assert.AreEqual("Alternative", sorted[0].Genre.Name);
            }
        }
    }
}
