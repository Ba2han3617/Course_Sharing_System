using System.ComponentModel.DataAnnotations;

namespace CourseNoteSharingSystem.ViewModels
{
    public class CourseListViewModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCourseViewModel
    {
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
    }

    public class EditCourseViewModel
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
    }
}
