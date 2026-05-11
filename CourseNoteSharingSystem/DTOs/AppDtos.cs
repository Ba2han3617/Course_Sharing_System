using System.ComponentModel.DataAnnotations;

namespace CourseNoteSharingSystem.DTOs
{
    public class RegisterDto
    {
        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Department { get; set; }
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class UpdateProfileDto
    {
        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Department { get; set; }

        [DataType(DataType.Password), MinLength(6)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }

    public class UploadNoteDto
    {
        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }

    public class EditNoteDto
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public int CourseId { get; set; }
    }

    public class AddCommentDto
    {
        [Required, MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int NoteId { get; set; }
    }

    public class RateNoteDto
    {
        [Required, Range(1, 5)]
        public int Score { get; set; }

        [Required]
        public int NoteId { get; set; }
    }
}
