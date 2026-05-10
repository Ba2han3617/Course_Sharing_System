using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseNoteSharingSystem.Models
{
    /// <summary>
    /// Represents an uploaded course note file.
    /// </summary>
    public class Note
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required, MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string FileType { get; set; } = string.Empty; // pdf, docx, pptx, zip

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public int DownloadCount { get; set; } = 0;

        // Foreign keys
        public int UserId { get; set; }
        public int CourseId { get; set; }

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
