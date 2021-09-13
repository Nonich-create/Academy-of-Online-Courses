using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Students.DAL.Models
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Manager> Manager { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<ApplicationCourse> ApplicationCourses { get; set; }
        public DbSet<LessonPlan> LessonPlans { set; get; }

        public Context(DbContextOptions<Context> options)
        : base(options)
        {
          //Database.EnsureDeleted();
          //Database.EnsureCreated();
        }
    } 
}
    