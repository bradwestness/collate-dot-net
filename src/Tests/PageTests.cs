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
                PageNumber = 2,
                PageSize = 5
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Students.OrderBy(x => x.Id).ToList();
                var paged = dbContext.Students
                    .OrderBy(x => x.Id) // needs to be ordered to apply paging
                    .Page(request)
                    .ToList();

                Assert.AreNotEqual(items.First().Id, paged.First().Id);
                Assert.AreEqual(items.Skip((request.PageNumber - 1) * request.PageSize).First().Id, paged.First().Id);
                Assert.AreNotEqual(request.PageSize, items.Count);
                Assert.AreEqual(request.PageSize, paged.Count);
            }
        }
    }
}
