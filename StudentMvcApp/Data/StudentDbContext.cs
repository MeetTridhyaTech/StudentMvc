using Microsoft.EntityFrameworkCore;
using StudentMvcApp.Models;

namespace StudentMvcApp.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options)
            : base(options)
        { }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasMany(c => c.SubCourses)
                .WithOne(c => c.ParentCourse)
                .HasForeignKey(c => c.ParentCourseId)
                .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(modelBuilder);

        }



    }
}
