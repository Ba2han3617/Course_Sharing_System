namespace CourseNoteSharingSystem.ViewModels
{
    /// <summary>
    /// ViewModel for the Note detail page.
    /// </summary>
    public class NoteDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public int DownloadCount { get; set; }
        public string UploaderName { get; set; } = string.Empty;
        public string UploaderId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; }
        public int? CurrentUserRating { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new();
    }

    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ViewModel for note cards on listing/search pages.
    /// </summary>
    public class NoteCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public int DownloadCount { get; set; }
        public int LikeCount { get; set; }
        public double AverageRating { get; set; }
        public string UploaderName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel for the home page.
    /// </summary>
    public class HomeViewModel
    {
        public List<NoteCardViewModel> PopularNotes { get; set; } = new();
        public List<NoteCardViewModel> RecentNotes { get; set; } = new();
        public List<TopContributorViewModel> TopContributors { get; set; } = new();
        public List<string> Departments { get; set; } = new();
        public int TotalNotes { get; set; }
        public int TotalUsers { get; set; }
        public int TotalDownloads { get; set; }
    }

    public class TopContributorViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public string? Department { get; set; }
        public int NoteCount { get; set; }
    }

    /// <summary>
    /// ViewModel for the user profile page.
    /// </summary>
    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? ProfileImage { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<NoteCardViewModel> UploadedNotes { get; set; } = new();
        public List<NoteCardViewModel> LikedNotes { get; set; } = new();
        public List<NoteCardViewModel> FavoriteNotes { get; set; } = new();
    }

    /// <summary>
    /// ViewModel for admin dashboard.
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalNotes { get; set; }
        public int TotalDownloads { get; set; }
        public int TotalComments { get; set; }
        public List<NoteCardViewModel> MostDownloadedNotes { get; set; } = new();
        public List<TopContributorViewModel> MostActiveUsers { get; set; } = new();
        public List<MonthlyStatViewModel> MonthlyUploads { get; set; } = new();
    }

    public class MonthlyStatViewModel
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    /// <summary>
    /// ViewModel for the notes search/filter page.
    /// </summary>
    public class NoteSearchViewModel
    {
        public string? SearchQuery { get; set; }
        public int? CourseId { get; set; }
        public string? Department { get; set; }
        public string? FileType { get; set; }
        public string SortBy { get; set; } = "newest";
        public List<NoteCardViewModel> Notes { get; set; } = new();
        public List<CourseSelectItem> Courses { get; set; } = new();
        public List<string> Departments { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class CourseSelectItem
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel for the admin users list page. Includes role from Identity.
    /// </summary>
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    /// <summary>
    /// ViewModel for administrative login.
    /// </summary>
    public class LoginViewModel
    {
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
