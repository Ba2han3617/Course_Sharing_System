using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseNoteSharingSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public int NoteId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(NoteId))]
        public Note Note { get; set; } = null!;
    }

    public class Like
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int NoteId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(NoteId))]
        public Note Note { get; set; } = null!;
    }

    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int NoteId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(NoteId))]
        public Note Note { get; set; } = null!;
    }

    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int NoteId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(NoteId))]
        public Note Note { get; set; } = null!;
    }
}
