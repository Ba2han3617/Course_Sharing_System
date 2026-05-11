using CourseNoteSharingSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseNoteSharingSystem.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Rating> Ratings => Set<Rating>();
        public DbSet<Favorite> Favorites => Set<Favorite>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Default Identity table names (AspNetUsers, AspNetRoles, etc.) are used here.

            modelBuilder.Entity<User>(e =>
            {
                e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Course>(e =>
            {
                e.HasIndex(c => c.Code).IsUnique();
                e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
                e.Property(c => c.UpdatedAt).HasDefaultValueSql("now()");
                
                e.HasData(
                    new Course { Id = 1, CourseName = "Data Structures and Algorithms", Code = "CENG201", Department = "Computer Engineering", Semester = "Fall", Description = "Fundamental data structures and algorithms." },
                    new Course { Id = 2, CourseName = "Database Management Systems", Code = "CENG301", Department = "Computer Engineering", Semester = "Spring", Description = "Relational databases and SQL." },
                    new Course { Id = 3, CourseName = "Operating Systems", Code = "CENG302", Department = "Computer Engineering", Semester = "Fall", Description = "OS concepts and design." },
                    new Course { Id = 4, CourseName = "Computer Networks", Code = "CENG401", Department = "Computer Engineering", Semester = "Fall", Description = "Network protocols and architecture." },
                    new Course { Id = 5, CourseName = "Software Engineering", Code = "CENG402", Department = "Computer Engineering", Semester = "Spring", Description = "Software development lifecycle." },
                    new Course { Id = 6, CourseName = "Calculus I", Code = "MATH101", Department = "Mathematics", Semester = "Fall", Description = "Differential and integral calculus." },
                    new Course { Id = 7, CourseName = "Linear Algebra", Code = "MATH102", Department = "Mathematics", Semester = "Spring", Description = "Vector spaces and linear transformations." },
                    new Course { Id = 8, CourseName = "Physics I", Code = "PHYS101", Department = "Physics", Semester = "Fall", Description = "Mechanics and thermodynamics." },
                    new Course { Id = 9, CourseName = "Introduction to Programming", Code = "CENG101", Department = "Computer Engineering", Semester = "Fall", Description = "Basic programming concepts." },
                    new Course { Id = 10, CourseName = "Artificial Intelligence", Code = "CENG403", Department = "Computer Engineering", Semester = "Spring", Description = "AI fundamentals and machine learning." }
                );
            });

            modelBuilder.Entity<Note>(e =>
            {
                e.Property(n => n.UploadDate).HasDefaultValueSql("now()");
                e.HasOne(n => n.User).WithMany(u => u.Notes).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(n => n.Course).WithMany(c => c.Notes).HasForeignKey(n => n.CourseId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Comment>(e =>
            {
                e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
                e.HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Like>(e =>
            {
                e.HasIndex(l => new { l.UserId, l.NoteId }).IsUnique();
            });

            modelBuilder.Entity<Rating>(e =>
            {
                e.HasIndex(r => new { r.UserId, r.NoteId }).IsUnique();
            });

            modelBuilder.Entity<Favorite>(e =>
            {
                e.HasIndex(f => new { f.UserId, f.NoteId }).IsUnique();
            });
        }
    }
}
