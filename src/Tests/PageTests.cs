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
                PageSize = 1
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Courses;
                var total = items.Count();
                var paged = dbContext.Courses.OrderBy(x => x.Title).Page(request);

                Assert.AreEqual(request.PageSize, paged.Count());
                Assert.AreNotEqual(request.PageSize, total);
                Assert.AreEqual("Biology", paged.First().Title);
            }
        }
    }
}
