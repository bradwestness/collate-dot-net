using Collate.Implementation;
using Collate.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;

namespace Collate.Tests
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

        [TestMethod]
        public void NestedNavigationSortTest()
        {
            var navigationSort = new Sort
            {
                Field = nameof(Genre.Name),
                Direction = SortDirection.Ascending
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext
                    .InvoiceLines
                    .Include("Track.Genre")
                    .ToList();
                var queryable = dbContext.InvoiceLines.NavigationSort(navigationSort, "Track", "Genre");
                var sql = ""; // ((DbQuery<Track>)queryable).Sql;
                var sorted = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(items.Count, sorted.Count);
                Assert.AreNotEqual(items[0].Track.Genre.Name, sorted[0].Track.Genre.Name);
                Assert.AreEqual("Alternative", sorted[0].Track.Genre.Name);
            }
        }

        [TestMethod]
        public void MultiSortTest()
        {
            var request = new SortRequest
            {
                Sorts = new Sort[]
                 {
                     new Sort { Direction = SortDirection.Ascending, Field = nameof(Track.AlbumId)},
                     new Sort { Direction = SortDirection.Descending, Field = nameof(Track.UnitPrice)}
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

                var albumId = int.MinValue;
                foreach (var album in sorted.GroupBy(x => x.AlbumId))
                {
                    Assert.IsFalse(albumId > album.Key);
                    albumId = album.Key;

                    var unitPrice = decimal.MaxValue;
                    foreach (var track in album)
                    {
                        Assert.IsFalse(unitPrice < track.UnitPrice);
                        unitPrice = track.UnitPrice;
                    }
                }
            }
        }

        [TestMethod]
        public void MultiSortExtensionTest()
        {
            var request = $"{nameof(Track.AlbumId)},-{nameof(Track.UnitPrice)}".ToSortRequest();

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Sort(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var sorted = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(items.Count, sorted.Count);

                var albumId = int.MinValue;
                foreach (var album in sorted.GroupBy(x => x.AlbumId))
                {
                    Assert.IsFalse(albumId > album.Key);
                    albumId = album.Key;

                    var unitPrice = decimal.MaxValue;
                    foreach (var track in album)
                    {
                        Assert.IsFalse(unitPrice < track.UnitPrice);
                        unitPrice = track.UnitPrice;
                    }
                }
            }
        }
    }
}
