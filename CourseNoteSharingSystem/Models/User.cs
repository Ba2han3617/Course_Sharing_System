using System.ComponentModel.DataAnnotations;

namespace CourseNoteSharingSystem.Models
{
    /// <summary>
    /// Represents a user (Student or Admin) in the system.
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(256), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(500)]
        public string? ProfileImage { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Student"; // Student | Admin

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
