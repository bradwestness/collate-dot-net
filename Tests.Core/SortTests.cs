﻿using Collate.Implementation;
using Collate.Tests.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;

namespace Collate.Tests.Core
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
                var sql = ""; // ((DbSet<Track>)queryable).Sql;
                var sorted = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(items.Count, sorted.Count);
                Assert.AreNotEqual(items[0].Name, sorted[0].Name);
                Assert.AreEqual("\"40\"", sorted[0].Name);
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
                var items = dbContext
                    .Tracks
                    .Include(x => x.Genre)
                    .ToList();
                var queryable = dbContext.Tracks.NavigationSort(navigationSort, nameof(Track.Genre));
                var sql = ""; // ((DbQuery<Track>)queryable).Sql;
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
                    .Include(x => x.Track)
                    .ThenInclude(x => x.Genre)
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
                     new Sort { Direction = SortDirection.Descending, Field = nameof(Track.Bytes)}
                 }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks
                    .Include(x => x.Album)
                    .ToList();
                var queryable = dbContext.Tracks.Sort(request);
                var sql = ""; // ((DbQuery<Track>)queryable).Sql;
                var sorted = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(items.Count, sorted.Count);

                var albumId = int.MinValue;
                foreach (var album in sorted.GroupBy(x => x.AlbumId))
                {
                    Assert.IsFalse(albumId > album.Key);
                    albumId = album.Key;

                    var bytes = decimal.MaxValue;
                    foreach (var track in album)
                    {
                        Assert.IsFalse(bytes < track.Bytes);
                        bytes = track.Bytes;
                    }
                }
            }
        }

        [TestMethod]
        public void MultiSortExtensionTest()
        {
            var request = $"{nameof(Track.AlbumId)},-{nameof(Track.Bytes)}".ToSortRequest();

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks
                    .Include(x => x.Album)
                    .ToList();
                var queryable = dbContext.Tracks.Sort(request);
                var sql = ""; // ((DbQuery<Track>)queryable).Sql;
                var sorted = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(items.Count, sorted.Count);

                var albumId = int.MinValue;
                foreach (var album in sorted.GroupBy(x => x.AlbumId))
                {
                    Assert.IsFalse(albumId > album.Key);
                    albumId = album.Key;

                    var bytes = decimal.MaxValue;
                    foreach (var track in album)
                    {
                        Assert.IsFalse(bytes < track.Bytes);
                        bytes = track.Bytes;
                    }
                }
            }
        }
    }
}
