using System.ComponentModel.DataAnnotations;

namespace CourseNoteSharingSystem.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required, MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Semester { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
