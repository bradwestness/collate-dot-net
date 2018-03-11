using Collate;
using Collate.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Data;

namespace Tests
{
    [TestClass]
    public class FilterTests
    {
        [TestMethod]
        public void FilterTest()
        {
            var request = new FilterRequest
            {
                Logic = FilterLogic.And,
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Course.Title),
                        Operator = FilterOperator.EndsWith,
                        Value = "y"
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Courses.ToList();
                var filtered = dbContext.Courses.Filter(request).ToList();

                Assert.AreNotEqual(items.Count(), filtered.Count());
                Assert.AreEqual(items.First().Title, filtered.First().Title);
                Assert.AreNotEqual(items.Last().Title, filtered.Last().Title);
            }
        }
    }
}
