using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Tests.Data
{
    public class TestDataContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<Student> Students { get; set; }

        public TestDataContext() : base("TestData")
        {
            Database.SetInitializer(new TestDataInitializer());
        }
    }

    public class TestDataInitializer : DropCreateDatabaseAlways<TestDataContext>
    {
        protected override void Seed(TestDataContext context)
        {
            var instructors = new List<Instructor>
                {
                    new Instructor
                    {
                        FirstName = "Dean",
                        LastName = "Pelton"
                    },
                    new Instructor
                    {
                        FirstName = "Ben",
                        LastName = "Chang"
                    },
                    new Instructor
                    {
                        FirstName = "Ian",
                        LastName = "Duncan"
                    },
                    new Instructor
                    {
                        FirstName = "Frankie",
                        LastName = "Dart"
                    },
                    new Instructor
                    {
                        FirstName = "Buzz",
                        LastName = "Hickey"
                    }
                };
            context.Instructors.AddRange(instructors);

            var courses = new List<Course>
                {
                    new Course
                    {
                        Title = "Spanish",
                        Instructor = instructors.First(x => x.LastName == "Chang")
                    },
                    new Course
                    {
                        Title = "Psychology",
                        Instructor = instructors.First(x => x.LastName == "Duncan")
                    },
                    new Course
                    {
                        Title = "Chemistry",
                        Instructor = instructors.First(x => x.LastName == "Pelton")
                    },
                    new Course
                    {
                        Title = "Algebra",
                        Instructor = instructors.First(x => x.LastName == "Dart")
                    },
                    new Course
                    {
                        Title = "Physical Education",
                        Instructor = instructors.First(x => x.LastName == "Hickey")
                    }
                };
            context.Courses.AddRange(courses);

            var students = new List<Student>
                {
                    new Student
                    {
                        FirstName = "Jeff",
                        LastName = "Winger",
                        Courses = courses.Where(x => x.Title.Contains("e")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Britta",
                        LastName = "Perry",
                        Courses = courses.Where(x => x.Title.Contains("y")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Abed",
                        LastName = "Nadir",
                        Courses = courses.Where(x => x.Title.Contains("a")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Annie",
                        LastName = "Edison",
                        Courses = courses.Where(x => x.Title == "Spanish").ToList()
                    },
                    new Student
                    {
                        FirstName = "Shirley",
                        LastName = "Bennett",
                        Courses = courses.Where(x => x.Title.EndsWith("y")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Troy",
                        LastName = "Barnes",
                        Courses = courses.Where(x => x.Title.StartsWith("P")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Pierce",
                        LastName = "Hawthorne",
                        Courses = courses.Where(x => x.Title.Contains("e")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Elroy",
                        LastName = "Patashnick",
                        Courses = courses.Where(x => x.Title.Contains("y")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Leonard",
                        LastName = "???",
                        Courses = courses.Where(x => x.Title.Contains("P")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Garrett",
                        LastName = "Lambert",
                        Courses = courses.Where(x => x.Title.Contains("a")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Alex",
                        LastName = "Star-Burns",
                        Courses = courses.Where(x => x.Title.Contains("e")).ToList()
                    }
                };
            context.Students.AddRange(students);

            base.Seed(context);
        }
    }

    public class Course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        public virtual Instructor Instructor { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }

    public class Instructor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }

    public class Student
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}
