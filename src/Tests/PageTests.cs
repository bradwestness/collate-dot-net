using Collate;
using Collate.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Data;

namespace Tests
{
    [TestClass]
    public class PageTests
    {
        [TestMethod]
        public void PageTest()
        {
            var request = new PageRequest
            {
                PageNumber = 5,
                PageSize = 25
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.OrderBy(x => x.TrackId).ToList();
                var paged = dbContext.Tracks
                    .OrderBy(x => x.TrackId) // needs to be ordered to apply paging
                    .Page(request)
                    .ToList();

                Assert.AreNotEqual(items.First().TrackId, paged.First().TrackId);
                Assert.AreEqual(items.Skip((request.PageNumber - 1) * request.PageSize).First().TrackId, paged.First().TrackId);
                Assert.AreNotEqual(request.PageSize, items.Count);
                Assert.AreEqual(request.PageSize, paged.Count);
            }
        }
    }
}
