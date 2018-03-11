using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                        FirstName = "Jack",
                        LastName = "Donaghy"
                    },
                    new Instructor
                    {
                        FirstName = "Elizabeth",
                        LastName = "Lemon"
                    }
                };
            context.Instructors.AddRange(instructors);

            var courses = new List<Course>
                {
                    new Course
                    {
                        Title = "Biology",
                        Instructor = instructors.First(x => x.FirstName == "Jack")
                    },
                    new Course
                    {
                        Title = "Chemistry",
                        Instructor = instructors.First(x => x.FirstName == "Elizabeth")
                    },
                    new Course
                    {
                        Title = "Algebra",
                        Instructor = instructors.First(x => x.FirstName == "Elizabeth")
                    }
                };
            context.Courses.AddRange(courses);

            var students = new List<Student>
                {
                    new Student
                    {
                        FirstName = "Tracy",
                        LastName = "Jordan",
                        Courses = courses.Where(x => x.Title.EndsWith("y")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Jenna",
                        LastName = "Maroney",
                        Courses = courses.Where(x => x.Title.Contains("e")).ToList()
                    },
                    new Student
                    {
                        FirstName = "Frank",
                        LastName = "Rossitano",
                        Courses = courses.Where(x => x.Title.Equals("Biology")).ToList()
                    }
                };
            context.Students.AddRange(students);

            base.Seed(context);
        }
    }

    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }

        public virtual Instructor Instructor { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }

    public class Instructor
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }

    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}
