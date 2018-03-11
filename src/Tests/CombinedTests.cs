using Collate;
using Collate.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        Field = nameof(Student.LastName),
                        Operator = FilterOperator.Contains,
                        Value = "e"
                    },
                    new Filter
                    {
                        Field = nameof(Student.FirstName),
                        Operator = FilterOperator.StartsWith,
                        Value = "F"
                    }
                },
                Sorts = new ISort[]
                {
                    new Sort
                    {
                        Field = nameof(Student.FirstName),
                        Direction = SortDirection.Descending
                    },
                    new Sort
                    {
                        Field = nameof(Student.LastName),
                        Direction = SortDirection.Ascending
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Students.ToList();
                var filtered = dbContext.Students.Filter(request);
                var paged = filtered
                    .Sort(request)
                    .Page(request);
                var list = paged.ToList();


            }
        }
    }
}
