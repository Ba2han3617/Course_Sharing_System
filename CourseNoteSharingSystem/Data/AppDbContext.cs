using CourseNoteSharingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseNoteSharingSystem.Data
{
    /// <summary>
    /// Entity Framework Core DbContext. All schema is managed via EF Core migrations only.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Rating> Ratings => Set<Rating>();
        public DbSet<Favorite> Favorites => Set<Favorite>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── User ────────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Role).HasDefaultValue("Student");
                e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
            });

            // ─── Course ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Course>(e =>
            {
                e.HasIndex(c => new { c.CourseName, c.Department }).IsUnique();
            });

            // ─── Note ────────────────────────────────────────────────────────────
            modelBuilder.Entity<Note>(e =>
            {
                e.Property(n => n.UploadDate).HasDefaultValueSql("now()");
                e.Property(n => n.DownloadCount).HasDefaultValue(0);

                // A note is deleted when its owner is deleted
                e.HasOne(n => n.User)
                 .WithMany(u => u.Notes)
                 .HasForeignKey(n => n.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Restrict deletion of a course that has notes
                e.HasOne(n => n.Course)
                 .WithMany(c => c.Notes)
                 .HasForeignKey(n => n.CourseId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── Comment ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Comment>(e =>
            {
                e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");

                e.HasOne(c => c.User)
                 .WithMany(u => u.Comments)
                 .HasForeignKey(c => c.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(c => c.Note)
                 .WithMany(n => n.Comments)
                 .HasForeignKey(c => c.NoteId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Like ────────────────────────────────────────────────────────────
            modelBuilder.Entity<Like>(e =>
            {
                // A user can like a specific note only once
                e.HasIndex(l => new { l.UserId, l.NoteId }).IsUnique();

                e.HasOne(l => l.User)
                 .WithMany(u => u.Likes)
                 .HasForeignKey(l => l.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(l => l.Note)
                 .WithMany(n => n.Likes)
                 .HasForeignKey(l => l.NoteId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Rating ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Rating>(e =>
            {
                // A user can rate a specific note only once
                e.HasIndex(r => new { r.UserId, r.NoteId }).IsUnique();

                e.HasOne(r => r.User)
                 .WithMany(u => u.Ratings)
                 .HasForeignKey(r => r.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(r => r.Note)
                 .WithMany(n => n.Ratings)
                 .HasForeignKey(r => r.NoteId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Favorite ────────────────────────────────────────────────────────
            modelBuilder.Entity<Favorite>(e =>
            {
                // A user can favorite a note only once
                e.HasIndex(f => new { f.UserId, f.NoteId }).IsUnique();

                e.HasOne(f => f.User)
                 .WithMany(u => u.Favorites)
                 .HasForeignKey(f => f.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(f => f.Note)
                 .WithMany(n => n.Favorites)
                 .HasForeignKey(f => f.NoteId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Seed Data ───────────────────────────────────────────────────────
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed admin user (password: Admin@123)
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "System Administrator",
                Email = "admin@cnss.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Department = "Computer Engineering",
                Role = "Admin",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Seed sample courses
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, CourseName = "Data Structures and Algorithms", Department = "Computer Engineering" },
                new Course { Id = 2, CourseName = "Database Management Systems", Department = "Computer Engineering" },
                new Course { Id = 3, CourseName = "Operating Systems", Department = "Computer Engineering" },
                new Course { Id = 4, CourseName = "Computer Networks", Department = "Computer Engineering" },
                new Course { Id = 5, CourseName = "Software Engineering", Department = "Computer Engineering" },
                new Course { Id = 6, CourseName = "Calculus I", Department = "Mathematics" },
                new Course { Id = 7, CourseName = "Linear Algebra", Department = "Mathematics" },
                new Course { Id = 8, CourseName = "Physics I", Department = "Physics" },
                new Course { Id = 9, CourseName = "Introduction to Programming", Department = "Computer Engineering" },
                new Course { Id = 10, CourseName = "Artificial Intelligence", Department = "Computer Engineering" }
            );
        }
    }
}
