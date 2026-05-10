using System.ComponentModel.DataAnnotations;

namespace CourseNoteSharingSystem.Models
{
    /// <summary>
    /// Represents a university course that notes can be uploaded for.
    /// </summary>
    public class Course
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        // Navigation
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
