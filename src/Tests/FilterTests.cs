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
    public class FilterTests
    {
        [TestMethod]
        public void SingleFilterTest()
        {
            var request = new FilterRequest
            {
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

            using (var DbQuery = new TestDataContext())
            {
                var items = DbQuery.Tracks.ToList();
                var queryable = DbQuery.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => !x.Name.EndsWith("y")));

                // assert that every item in the set matches the filter
                Assert.IsFalse(filtered.All(x => x.Name.EndsWith("y")));
            }
        }

        [TestMethod]
        public void MultiFilterAndTest()
        {
            var request = new FilterRequest
            {
                Logic = FilterLogic.And,
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.StartsWith,
                        Value = "C"
                    },
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.EndsWith,
                        Value = "y"
                    }
                }
            };

            using (var DbQuery = new TestDataContext())
            {
                var items = DbQuery.Tracks.ToList();
                var queryable = DbQuery.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsFalse(items.All(x => x.Name.StartsWith("C")));
                Assert.IsFalse(items.All(x => x.Name.EndsWith("y")));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Name.StartsWith("C")));
                Assert.IsTrue(filtered.All(x => x.Name.EndsWith("y")));
            }
        }

        [TestMethod]
        public void MultiFilterOrTest()
        {
            var request = new FilterRequest
            {
                Logic = FilterLogic.Or,
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.StartsWith,
                        Value = "C"
                    },
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.EndsWith,
                        Value = "y"
                    }
                }
            };

            using (var DbQuery = new TestDataContext())
            {
                var items = DbQuery.Tracks.ToList();
                var queryable = DbQuery.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsFalse(items.All(x => x.Name.StartsWith("C") || x.Name.EndsWith("y")));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Name.StartsWith("C") || x.Name.EndsWith("y")));

                // assert that every item in the list didn't fulfill both criteria
                Assert.IsFalse(filtered.All(x => x.Name.StartsWith("C") && x.Name.EndsWith("y")));
            }
        }
    }
}
