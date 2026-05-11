using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CourseNoteSharingSystem.Models
{
    public class User : IdentityUser
    {
        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(500)]
        public string? ProfileImage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
