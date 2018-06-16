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
        public void EqualTest()
        {
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.Equal,
                        Value = "Crazy"
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => !x.Name.Equals("Crazy")));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Name.Equals("Crazy")));
            }
        }

        [TestMethod]
        public void NotEqualTest()
        {
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.NotEqual,
                        Value = "Crazy"
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there was at least one item in the list that equaled the fitler value
                Assert.IsTrue(items.Any(x => x.Name.Equals("Crazy")));

                // assert that no items in the fitlered set match the filter
                Assert.IsTrue(filtered.All(x => !x.Name.Equals("Crazy")));
            }
        }

        [TestMethod]
        public void ContainsTest()
        {
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.Contains,
                        Value = "Crazy"
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => !x.Name.Contains("Crazy")));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Name.Contains("Crazy")));

                // assert that every item in the filtered set does not equal the filter
                // (they should contain it but not equal it, necessarily)
                Assert.IsFalse(filtered.All(x => x.Name.Equals("Crazy")));
            }
        }

        [TestMethod]
        public void DoesNotContainTest()
        {
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.DoesNotContain,
                        Value = "Crazy"
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that match the filter
                Assert.IsTrue(items.Any(x => x.Name.Contains("Crazy")));

                // assert that every no item in the filtered set matches the filter
                Assert.IsTrue(filtered.All(x => !x.Name.Contains("Crazy")));
            }
        }

        [TestMethod]
        public void EndsWithTest()
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

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => !x.Name.EndsWith("y")));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Name.EndsWith("y")));
            }
        }

        [TestMethod]
        public void StartsWithTest()
        {
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Name),
                        Operator = FilterOperator.StartsWith,
                        Value = "Y"
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => !x.Name.StartsWith("Y")));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Name.StartsWith("Y")));
            }
        }

        [TestMethod]
        public void GreaterThanOrEqualTest()
        {
            var bytes = 34_618_222;
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Bytes),
                        Operator = FilterOperator.GreaterThanOrEqual,
                        Value = bytes.ToString()
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => x.Bytes <= bytes));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Bytes >= bytes));
            }
        }

        [TestMethod]
        public void LessThanOrEqualTest()
        {
            var bytes = 34_618_222;
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Bytes),
                        Operator = FilterOperator.LessThanOrEqual,
                        Value = bytes.ToString()
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => x.Bytes >= bytes));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Bytes <= bytes));
            }
        }

        [TestMethod]
        public void LessThanTest()
        {
            var bytes = 34_618_222;
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Bytes),
                        Operator = FilterOperator.LessThan,
                        Value = bytes.ToString()
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => x.Bytes > bytes));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Bytes < bytes));
            }
        }

        [TestMethod]
        public void GreaterThanTest()
        {
            var bytes = 34_618_222;
            var request = new FilterRequest
            {
                Filters = new IFilter[]
                {
                    new Filter
                    {
                        Field = nameof(Track.Bytes),
                        Operator = FilterOperator.GreaterThan,
                        Value = bytes.ToString()
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsTrue(items.Any(x => x.Bytes < bytes));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Bytes > bytes));
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

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
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

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
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

        [TestMethod]
        public void MultiFilterExtensionTest()
        {
            var request = new[] { "A", "B", "C" }.ToFilterRequest(nameof(Track.Name), FilterOperator.StartsWith, FilterLogic.Or);
            
            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Tracks.ToList();
                var queryable = dbContext.Tracks.Filter(request);
                var sql = ((DbQuery<Track>)queryable).Sql;
                var filtered = queryable.ToList();

                Debug.WriteLine(sql);

                // assert that there were some items in the full set that didn't match the filter
                Assert.IsFalse(items.All(x => x.Name.StartsWith("A") || x.Name.StartsWith("B") || x.Name.StartsWith("C")));

                // assert that every item in the set matches the filter
                Assert.IsTrue(filtered.All(x => x.Name.StartsWith("A") || x.Name.StartsWith("B") ||  x.Name.StartsWith("C")));

                // assert that every item in the list didn't fulfill both criteria
                Assert.IsFalse(filtered.All(x => x.Name.StartsWith("A") && x.Name.StartsWith("B") && x.Name.StartsWith("C")));
            }
        }
    }
}
