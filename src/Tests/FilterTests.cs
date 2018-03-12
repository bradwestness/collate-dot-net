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

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => x.Title.EndsWith("y")));

                // assert that every item in the set matches the filter
                Assert.IsFalse(filtered.Any(x => !x.Title.EndsWith("y")));
            }
        }
    }
}
