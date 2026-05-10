using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Helpers;
using CourseNoteSharingSystem.Models;
using CourseNoteSharingSystem.Repositories;
using CourseNoteSharingSystem.ViewModels;

namespace CourseNoteSharingSystem.Services
{
    // ─── Auth Service ─────────────────────────────────────────────────────────
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;

        public AuthService(IUserRepository users) => _users = users;

        public async Task<(bool Success, string? Error)> RegisterAsync(RegisterDto dto)
        {
            if (await _users.GetByEmailAsync(dto.Email) != null)
                return (false, "This email is already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Department = dto.Department,
                Role = "Student"
            };

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();
            return (true, null);
        }

        public async Task<User?> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);
            if (user == null) return null;
            return BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash) ? user : null;
        }
    }

    // ─── Note Service ─────────────────────────────────────────────────────────
    public class NoteService : INoteService
    {
        private readonly INoteRepository _notes;
        private readonly ICourseRepository _courses;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExtensions = [".pdf", ".docx", ".pptx", ".zip"];
        private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB

        public NoteService(INoteRepository notes, ICourseRepository courses, IWebHostEnvironment env)
        {
            _notes = notes;
            _courses = courses;
            _env = env;
        }

        public async Task<NoteDetailViewModel?> GetNoteDetailAsync(int noteId, int? currentUserId)
        {
            var note = await _notes.GetWithDetailsAsync(noteId);
            if (note == null) return null;

            var avgRating = note.Ratings.Count == 0 ? 0 : note.Ratings.Average(r => r.Score);

            return new NoteDetailViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
                FilePath = note.FilePath,
                FileType = note.FileType,
                UploadDate = note.UploadDate,
                DownloadCount = note.DownloadCount,
                UploaderName = note.User.FullName,
                UploaderId = note.UserId,
                CourseName = note.Course.CourseName,
                CourseId = note.CourseId,
                AverageRating = Math.Round(avgRating, 1),
                RatingCount = note.Ratings.Count,
                LikeCount = note.Likes.Count,
                IsLikedByCurrentUser = currentUserId.HasValue && note.Likes.Any(l => l.UserId == currentUserId),
                IsFavoritedByCurrentUser = currentUserId.HasValue && note.Favorites.Any(f => f.UserId == currentUserId),
                CurrentUserRating = currentUserId.HasValue
                    ? note.Ratings.FirstOrDefault(r => r.UserId == currentUserId)?.Score
                    : null,
                Comments = note.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        Content = c.Content,
                        UserName = c.User.FullName,
                        UserAvatar = c.User.ProfileImage,
                        UserId = c.UserId,
                        CreatedAt = c.CreatedAt
                    }).ToList()
            };
        }

        public async Task<List<NoteCardViewModel>> GetRecentNotesAsync(int count)
            => (await _notes.GetRecentAsync(count)).Select(MapToCard).ToList();

        public async Task<List<NoteCardViewModel>> GetPopularNotesAsync(int count)
            => (await _notes.GetPopularAsync(count)).Select(MapToCard).ToList();

        public async Task<NoteSearchViewModel> SearchAsync(NoteSearchViewModel filter)
        {
            var notes = await _notes.SearchAsync(filter.SearchQuery, filter.CourseId,
                filter.Department, filter.FileType, filter.SortBy, filter.PageNumber, filter.PageSize);
            var total = await _notes.CountSearchAsync(filter.SearchQuery, filter.CourseId, filter.Department, filter.FileType);
            var courses = (await _courses.GetAllAsync()).Select(c => new CourseSelectItem
                { Id = c.Id, CourseName = c.CourseName, Department = c.Department }).ToList();

            filter.Notes = notes.Select(MapToCard).ToList();
            filter.TotalCount = total;
            filter.Courses = courses;
            filter.Departments = await _courses.GetDepartmentsAsync();
            return filter;
        }

        public async Task<(bool Success, string? Error, int NoteId)> UploadNoteAsync(UploadNoteDto dto, int userId)
        {
            var ext = Path.GetExtension(dto.File.FileName).ToLower();
            if (!AllowedExtensions.Contains(ext))
                return (false, "Only PDF, DOCX, PPTX, and ZIP files are allowed.", 0);
            if (dto.File.Length > MaxFileSizeBytes)
                return (false, "File size must be less than 50 MB.", 0);

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsPath, uniqueName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
                await dto.File.CopyToAsync(stream);

            var note = new Note
            {
                Title = dto.Title,
                Description = dto.Description,
                FilePath = $"/uploads/{uniqueName}",
                FileType = ext.TrimStart('.'),
                UserId = userId,
                CourseId = dto.CourseId
            };

            await _notes.AddAsync(note);
            await _notes.SaveChangesAsync();
            return (true, null, note.Id);
        }

        public async Task<(bool Success, string? Error)> EditNoteAsync(EditNoteDto dto, int userId)
        {
            var note = await _notes.GetByIdAsync(dto.Id);
            if (note == null) return (false, "Note not found.");
            if (note.UserId != userId) return (false, "Unauthorized.");

            note.Title = dto.Title;
            note.Description = dto.Description;
            note.CourseId = dto.CourseId;
            await _notes.UpdateAsync(note);
            await _notes.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteNoteAsync(int noteId, int userId, bool isAdmin)
        {
            var note = await _notes.GetByIdAsync(noteId);
            if (note == null) return (false, "Note not found.");
            if (!isAdmin && note.UserId != userId) return (false, "Unauthorized.");

            // Delete physical file
            var fullPath = Path.Combine(_env.WebRootPath, note.FilePath.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);

            await _notes.DeleteAsync(note);
            await _notes.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? FilePath, string? FileName, string? ContentType)> GetDownloadAsync(int noteId)
        {
            var note = await _notes.GetByIdAsync(noteId);
            if (note == null) return (false, null, null, null);

            var fullPath = Path.Combine(_env.WebRootPath, note.FilePath.TrimStart('/'));
            if (!File.Exists(fullPath)) return (false, null, null, null);

            await _notes.IncrementDownloadCountAsync(noteId);

            var contentType = note.FileType switch
            {
                "pdf" => "application/pdf",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "zip" => "application/zip",
                _ => "application/octet-stream"
            };

            var fileName = $"{note.Title}.{note.FileType}";
            return (true, fullPath, fileName, contentType);
        }

        private static NoteCardViewModel MapToCard(Note n) => new()
        {
            Id = n.Id,
            Title = n.Title,
            Description = n.Description,
            FileType = n.FileType,
            UploadDate = n.UploadDate,
            DownloadCount = n.DownloadCount,
            LikeCount = n.Likes?.Count ?? 0,
            AverageRating = n.Ratings?.Count > 0 ? Math.Round(n.Ratings.Average(r => r.Score), 1) : 0,
            UploaderName = n.User?.FullName ?? "",
            CourseName = n.Course?.CourseName ?? ""
        };
    }

    // ─── Interaction Service ──────────────────────────────────────────────────
    public class InteractionService : IInteractionService
    {
        private readonly ILikeRepository _likes;
        private readonly ICommentRepository _comments;
        private readonly IRatingRepository _ratings;
        private readonly IFavoriteRepository _favorites;

        public InteractionService(ILikeRepository likes, ICommentRepository comments,
            IRatingRepository ratings, IFavoriteRepository favorites)
        {
            _likes = likes;
            _comments = comments;
            _ratings = ratings;
            _favorites = favorites;
        }

        public async Task<(bool IsLiked, int LikeCount)> ToggleLikeAsync(int noteId, int userId)
        {
            var existing = await _likes.GetUserLikeAsync(userId, noteId);
            if (existing != null)
            {
                await _likes.DeleteAsync(existing);
                await _likes.SaveChangesAsync();
                return (false, await _likes.CountByNoteAsync(noteId));
            }

            await _likes.AddAsync(new Like { UserId = userId, NoteId = noteId });
            await _likes.SaveChangesAsync();
            return (true, await _likes.CountByNoteAsync(noteId));
        }

        public async Task<(bool Success, string? Error)> AddCommentAsync(AddCommentDto dto, int userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                return (false, "Comment cannot be empty.");

            await _comments.AddAsync(new Comment { Content = dto.Content, NoteId = dto.NoteId, UserId = userId });
            await _comments.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId, int userId, bool isAdmin)
        {
            var comment = await _comments.GetByIdAsync(commentId);
            if (comment == null) return (false, "Comment not found.");
            if (!isAdmin && comment.UserId != userId) return (false, "Unauthorized.");

            await _comments.DeleteAsync(comment);
            await _comments.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, double NewAverage, int Count)> RateNoteAsync(RateNoteDto dto, int userId)
        {
            var existing = await _ratings.GetUserRatingAsync(userId, dto.NoteId);
            if (existing != null)
            {
                existing.Score = dto.Score;
                await _ratings.UpdateAsync(existing);
            }
            else
            {
                await _ratings.AddAsync(new Rating { Score = dto.Score, NoteId = dto.NoteId, UserId = userId });
            }

            await _ratings.SaveChangesAsync();
            var avg = await _ratings.GetAverageAsync(dto.NoteId);
            var count = await _ratings.GetCountAsync(dto.NoteId);
            return (true, Math.Round(avg, 1), count);
        }

        public async Task<bool> ToggleFavoriteAsync(int noteId, int userId)
        {
            var existing = await _favorites.GetUserFavoriteAsync(userId, noteId);
            if (existing != null)
            {
                await _favorites.DeleteAsync(existing);
                await _favorites.SaveChangesAsync();
                return false;
            }

            await _favorites.AddAsync(new Favorite { UserId = userId, NoteId = noteId });
            await _favorites.SaveChangesAsync();
            return true;
        }
    }

    // ─── Profile Service ──────────────────────────────────────────────────────
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _users;
        private readonly INoteRepository _notes;
        private readonly IWebHostEnvironment _env;

        public ProfileService(IUserRepository users, INoteRepository notes, IWebHostEnvironment env)
        {
            _users = users;
            _notes = notes;
            _env = env;
        }

        public async Task<ProfileViewModel?> GetProfileAsync(int userId)
        {
            var user = await _users.GetWithDetailsAsync(userId);
            if (user == null) return null;

            return new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Department = user.Department,
                ProfileImage = user.ProfileImage,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UploadedNotes = (await _notes.GetByUserAsync(userId)).Select(n => new NoteCardViewModel
                {
                    Id = n.Id, Title = n.Title, FileType = n.FileType,
                    UploadDate = n.UploadDate, DownloadCount = n.DownloadCount,
                    LikeCount = n.Likes?.Count ?? 0, CourseName = n.Course?.CourseName ?? "",
                    AverageRating = n.Ratings?.Count > 0 ? Math.Round(n.Ratings.Average(r => r.Score), 1) : 0
                }).ToList(),
                LikedNotes = (await _notes.GetLikedByUserAsync(userId)).Select(n => new NoteCardViewModel
                {
                    Id = n.Id, Title = n.Title, FileType = n.FileType,
                    UploadDate = n.UploadDate, DownloadCount = n.DownloadCount,
                    LikeCount = n.Likes?.Count ?? 0, CourseName = n.Course?.CourseName ?? "",
                    UploaderName = n.User?.FullName ?? ""
                }).ToList(),
                FavoriteNotes = (await _notes.GetFavoritedByUserAsync(userId)).Select(n => new NoteCardViewModel
                {
                    Id = n.Id, Title = n.Title, FileType = n.FileType,
                    UploadDate = n.UploadDate, DownloadCount = n.DownloadCount,
                    LikeCount = n.Likes?.Count ?? 0, CourseName = n.Course?.CourseName ?? "",
                    UploaderName = n.User?.FullName ?? ""
                }).ToList()
            };
        }

        public async Task<(bool Success, string? Error)> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return (false, "User not found.");

            user.FullName = dto.FullName;
            user.Department = dto.Department;

            if (!string.IsNullOrEmpty(dto.NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var ext = Path.GetExtension(dto.ProfileImage.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
                    return (false, "Only JPG, PNG and WebP images are allowed for profile picture.");

                var avatarsPath = Path.Combine(_env.WebRootPath, "avatars");
                Directory.CreateDirectory(avatarsPath);

                var uniqueName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(avatarsPath, uniqueName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await dto.ProfileImage.CopyToAsync(stream);

                user.ProfileImage = $"/avatars/{uniqueName}";
            }

            await _users.UpdateAsync(user);
            await _users.SaveChangesAsync();
            return (true, null);
        }
    }

    // ─── Course Service ───────────────────────────────────────────────────────
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courses;

        public CourseService(ICourseRepository courses) => _courses = courses;

        public async Task<List<CourseSelectItem>> GetAllAsync()
            => (await _courses.GetAllAsync()).Select(c => new CourseSelectItem
                { Id = c.Id, CourseName = c.CourseName, Department = c.Department }).ToList();

        public async Task<List<string>> GetDepartmentsAsync()
            => await _courses.GetDepartmentsAsync();

        public async Task<List<CourseSelectItem>> SearchAsync(string? query, string? department)
            => (await _courses.SearchAsync(query, department)).Select(c => new CourseSelectItem
                { Id = c.Id, CourseName = c.CourseName, Department = c.Department }).ToList();
    }

    // ─── Admin Service ────────────────────────────────────────────────────────
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _users;
        private readonly INoteRepository _notes;
        private readonly ICommentRepository _comments;
        private readonly IWebHostEnvironment _env;

        public AdminService(IUserRepository users, INoteRepository notes,
            ICommentRepository comments, IWebHostEnvironment env)
        {
            _users = users;
            _notes = notes;
            _comments = comments;
            _env = env;
        }

        public async Task<AdminDashboardViewModel> GetDashboardAsync()
        {
            var allUsers = (await _users.GetAllAsync()).ToList();
            var allNotes = (await _notes.GetPopularAsync(1000)).ToList(); // get all via popular (sorted)
            var allComments = await _comments.GetByNoteAsync(0); // we'll fix this

            // Count totals via LINQ
            var totalDownloads = allNotes.Sum(n => n.DownloadCount);

            var topNotes = allNotes.Take(5).Select(n => new NoteCardViewModel
            {
                Id = n.Id, Title = n.Title, FileType = n.FileType,
                DownloadCount = n.DownloadCount, LikeCount = n.Likes?.Count ?? 0,
                UploaderName = n.User?.FullName ?? "", CourseName = n.Course?.CourseName ?? ""
            }).ToList();

            var topUsers = await _users.GetTopContributorsAsync(5);
            var topContributors = topUsers.Select(u => new TopContributorViewModel
            {
                UserId = u.Id, FullName = u.FullName, ProfileImage = u.ProfileImage,
                Department = u.Department, NoteCount = u.Notes?.Count ?? 0
            }).ToList();

            // Monthly uploads for the current year
            var currentYear = DateTime.UtcNow.Year;
            var monthlyUploads = allNotes
                .Where(n => n.UploadDate.Year == currentYear)
                .GroupBy(n => n.UploadDate.Month)
                .Select(g => new MonthlyStatViewModel
                {
                    Month = System.Globalization.CultureInfo.InvariantCulture
                        .DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                    Count = g.Count()
                })
                .OrderBy(m => Array.IndexOf(
                    System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames, m.Month))
                .ToList();

            return new AdminDashboardViewModel
            {
                TotalUsers = allUsers.Count,
                TotalNotes = allNotes.Count,
                TotalDownloads = totalDownloads,
                TotalComments = 0,
                MostDownloadedNotes = topNotes,
                MostActiveUsers = topContributors,
                MonthlyUploads = monthlyUploads
            };
        }

        public async Task<List<User>> GetAllUsersAsync() => (await _users.GetAllAsync()).ToList();

        public async Task<(bool Success, string? Error)> DeleteUserAsync(int userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return (false, "User not found.");
            if (user.Role == "Admin") return (false, "Cannot delete admin accounts.");
            await _users.DeleteAsync(user);
            await _users.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteNoteAsync(int noteId)
        {
            var note = await _notes.GetByIdAsync(noteId);
            if (note == null) return (false, "Note not found.");
            var fullPath = Path.Combine(_env.WebRootPath, note.FilePath.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
            await _notes.DeleteAsync(note);
            await _notes.SaveChangesAsync();
            return (true, null);
        }

        public async Task<List<Models.Comment>> GetAllCommentsAsync()
        {
            // Fetch comments for all notes by getting a flat list
            var notes = (await _notes.GetPopularAsync(10000)).ToList();
            var result = new List<Models.Comment>();
            foreach (var note in notes)
            {
                var comments = await _comments.GetByNoteAsync(note.Id);
                result.AddRange(comments);
            }
            return result.OrderByDescending(c => c.CreatedAt).ToList();
        }

        public async Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId)
        {
            var comment = await _comments.GetByIdAsync(commentId);
            if (comment == null) return (false, "Comment not found.");
            await _comments.DeleteAsync(comment);
            await _comments.SaveChangesAsync();
            return (true, null);
        }
    }
}
