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
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.EndsWith,
                        Value = "y"
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var filtered = dbContext.Tracks.Filter(request).ToList();

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => x.Name.EndsWith("y")));

                // assert that every item in the set matches the filter
                Assert.IsFalse(filtered.Any(x => !x.Name.EndsWith("y")));
            }
        }
    }
}
