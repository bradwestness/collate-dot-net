using Collate;
using Collate.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Data;

namespace Tests
{
    [TestClass]
    public class SortTests
    {
        [TestMethod]
        public void SortTest()
        {
            var request = new SortRequest
            {
                Sorts = new ISort[]
                {
                    new Sort
                    {
                        Field = nameof(Track.Name),
                        Direction = SortDirection.Ascending
                    },
                    new Sort
                    {
                        Field = nameof(Track.Milliseconds),
                        Direction = SortDirection.Ascending
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var sorted = dbContext.Tracks.Sort(request).ToList();

                Assert.AreEqual(items.Count, sorted.Count);
                Assert.AreNotEqual(items.First().Name, sorted.First().Name);
                Assert.AreEqual("'Round Midnight", sorted.First().Name);
            }
        }
    }
}
