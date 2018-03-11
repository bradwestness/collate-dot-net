using Collate;
using Collate.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Data;

namespace Tests
{
    [TestClass]
    public class SortTests
    {
        [TestMethod]
        public void SortTest()
        {
            var request = new SortRequest
            {
                Sorts = new ISort[]
                {
                    new Sort
                    {
                        Field = nameof(Student.LastName),
                        Direction = SortDirection.Descending
                    },
                    new Sort
                    {
                        Field = nameof(Student.FirstName),
                        Direction = SortDirection.Ascending
                    }
                }
            };

            using (var dbContext = new TestDataContext())
            {
                var items = dbContext.Students.ToList();
                var sorted = dbContext.Students.Sort(request).ToList();

                Assert.AreEqual(items.Count(), sorted.Count());
                Assert.AreNotEqual(items.First().LastName, sorted.First().LastName);
                Assert.AreEqual("Frank", sorted.First().FirstName);
            }
        }
    }
}
