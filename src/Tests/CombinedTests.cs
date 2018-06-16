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
    public class CombinedTests
    {
        [TestMethod]
        public void CombinedTest()
        {
            var request = new PageAndFilterAndSortRequest
            {
                PageNumber = 3,
                PageSize = 25,
                Logic = FilterLogic.And,
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.Contains,
                        Value = "e"
                    },
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.StartsWith,
                        Value = "F"
                    }
                },
                Sorts = new ISort[]
                {
                    new Sort
                    {
                        Field = nameof(Track.Milliseconds),
                        Direction = SortDirection.Descending
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var filtered = dbContext.Tracks.Filter(request);
                var queryable = filtered
                    .Sort(request)
                    .Page(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var list = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(request.PageSize, list.Count);
                foreach (var filter in request.Filters)
                {
                    switch (filter.Operator)
                    {
                        case FilterOperator.StartsWith:
                            Assert.IsTrue(list.All(x => x.Name.StartsWith(filter.Value)));
                            break;

                        case FilterOperator.Contains:
                            Assert.IsTrue(list.All(x => x.Name.Contains(filter.Value)));
                            break;
                    }
                }
                Assert.AreEqual(list.Max(x => x.Milliseconds), list.First().Milliseconds);
            }
        }

        [TestMethod]
        public void CustomerTest()
        {
            var request = new PageAndFilterAndSortRequest
            {
                PageNumber = 3,
                PageSize = 25,
                Logic = FilterLogic.And,
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Customer.FirstName),
                        Operator = FilterOperator.Contains,
                        Value = "Jane"
                    }
                },
                Sorts = new ISort[]
                {
                    new Sort
                    {
                        Field = nameof(Customer.LastName),
                        Direction = SortDirection.Ascending
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Customers.ToList();
                var filtered = dbContext.Customers.Filter(request);
                var queryable = filtered
                    .Sort(request)
                    .Page(request);
                var sql = ((DbQuery<Customer>)queryable).Sql;
                var list = queryable.ToList();

                Debug.WriteLine(sql);

                Assert.AreEqual(request.PageSize, list.Count);
            }
        }
    }
}
