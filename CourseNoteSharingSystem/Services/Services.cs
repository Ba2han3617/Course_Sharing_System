using System.Security.Claims;
using CourseNoteSharingSystem.DTOs;

using CourseNoteSharingSystem.Models;
using CourseNoteSharingSystem.Repositories;
using CourseNoteSharingSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;


namespace CourseNoteSharingSystem.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _notes;
        private readonly ICourseRepository _courses;
        private readonly IFavoriteRepository _favorites;
        public NoteService(INoteRepository notes, ICourseRepository courses, IFavoriteRepository favorites) 
        { 
            _notes = notes; 
            _courses = courses; 
            _favorites = favorites;
        }

        public async Task<NoteDetailViewModel?> GetNoteDetailAsync(int noteId, string? currentUserId)
        {
            var note = await _notes.GetWithDetailsAsync(noteId);
            if (note == null) return null;
            
            var avg = note.Ratings.Any() ? note.Ratings.Average(r => r.Score) : 0;
            
            return new NoteDetailViewModel 
            { 
                Id = note.Id, 
                Title = note.Title, 
                Description = note.Description,
                FilePath = note.FilePath,
                FileType = note.FileType,
                UploadDate = note.UploadDate,
                DownloadCount = note.DownloadCount,
                UploaderName = note.User?.FullName ?? "Unknown",
                UploaderId = note.UserId,
                CourseName = note.Course?.CourseName ?? "",
                CourseId = note.CourseId,
                AverageRating = Math.Round(avg, 1),
                RatingCount = note.Ratings.Count,
                LikeCount = note.Likes.Count,
                IsLikedByCurrentUser = currentUserId != null && note.Likes.Any(l => l.UserId == currentUserId),
                IsFavoritedByCurrentUser = currentUserId != null && _favorites != null && (await _favorites.GetUserFavoriteAsync(currentUserId, note.Id)) != null,
                CurrentUserRating = currentUserId != null ? note.Ratings.FirstOrDefault(r => r.UserId == currentUserId)?.Score : null,
                Comments = note.Comments.Select(c => new CommentViewModel
                {
                    Id = c.Id, Content = c.Content, UserName = c.User?.FullName ?? "User", UserId = c.UserId, CreatedAt = c.CreatedAt
                }).ToList()
            };
        }

        public async Task<List<NoteCardViewModel>> GetRecentNotesAsync(int count)
            => (await _notes.GetRecentAsync(count)).Select(MapToCard).ToList();

        public async Task<List<NoteCardViewModel>> GetPopularNotesAsync(int count)
            => (await _notes.GetPopularAsync(count)).Select(MapToCard).ToList();

        public async Task<NoteSearchViewModel> SearchAsync(NoteSearchViewModel filter)
        {
            var notes = await _notes.SearchAsync(filter.SearchQuery, filter.CourseId, filter.Department, filter.FileType, filter.SortBy, filter.PageNumber, filter.PageSize);
            var total = await _notes.CountSearchAsync(filter.SearchQuery, filter.CourseId, filter.Department, filter.FileType);
            
            filter.Notes = notes.Select(MapToCard).ToList();
            filter.TotalCount = total;
            filter.Courses = (await _courses.GetAllAsync()).Select(c => new CourseSelectItem { Id = c.Id, CourseName = c.CourseName, Department = c.Department }).ToList();
            filter.Departments = await _courses.GetDepartmentsAsync();
            return filter;
        }

        public async Task<(bool Success, string? Error, int NoteId)> UploadNoteAsync(UploadNoteDto dto, string userId)
        {
            var note = new Note
            {
                Title = dto.Title,
                Description = dto.Description,
                CourseId = dto.CourseId,
                UserId = userId,
                FileType = Path.GetExtension(dto.File.FileName).TrimStart('.').ToUpper(),
                FilePath = "/uploads/" + dto.File.FileName, // Placeholder
                UploadDate = DateTime.UtcNow
            };
            await _notes.AddAsync(note);
            await _notes.SaveChangesAsync();
            return (true, null, note.Id);
        }

        public async Task<(bool Success, string? Error)> EditNoteAsync(EditNoteDto dto, string userId)
        {
            var note = await _notes.GetByIdAsync(dto.Id);
            if (note == null || note.UserId != userId) return (false, "Note not found or access denied.");
            
            note.Title = dto.Title;
            note.Description = dto.Description;
            note.CourseId = dto.CourseId;
            
            await _notes.UpdateAsync(note);
            await _notes.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteNoteAsync(int noteId, string userId, bool isAdmin)
        {
            var note = await _notes.GetByIdAsync(noteId);
            if (note == null) return (false, "Note not found.");
            if (note.UserId != userId && !isAdmin) return (false, "Access denied.");
            
            await _notes.DeleteAsync(note);
            await _notes.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? FilePath, string? FileName, string? ContentType)> GetDownloadAsync(int noteId)
        {
            var note = await _notes.GetByIdAsync(noteId);
            if (note == null) return (false, null, null, null);
            
            note.DownloadCount++;
            await _notes.SaveChangesAsync();
            
            return (true, note.FilePath, note.Title, "application/octet-stream");
        }

        private NoteCardViewModel MapToCard(Note n) => new NoteCardViewModel
        {
            Id = n.Id, Title = n.Title, FileType = n.FileType, UploadDate = n.UploadDate,
            DownloadCount = n.DownloadCount, LikeCount = n.Likes?.Count ?? 0,
            AverageRating = n.Ratings != null && n.Ratings.Any() ? Math.Round(n.Ratings.Average(r => r.Score), 1) : 0,
            UploaderName = n.User?.FullName ?? "", CourseName = n.Course?.CourseName ?? ""
        };
    }

    public class InteractionService : IInteractionService
    {
        private readonly ILikeRepository _likes;
        private readonly IRatingRepository _ratings;
        private readonly IFavoriteRepository _favorites;
        private readonly ICommentRepository _comments;
        
        public InteractionService(ILikeRepository likes, IRatingRepository ratings, IFavoriteRepository favorites, ICommentRepository comments)
        {
            _likes = likes; _ratings = ratings; _favorites = favorites; _comments = comments;
        }

        public async Task<(bool IsLiked, int LikeCount)> ToggleLikeAsync(int noteId, string userId)
        {
            var like = await _likes.GetUserLikeAsync(userId, noteId);
            if (like != null) await _likes.DeleteAsync(like);
            else await _likes.AddAsync(new Like { UserId = userId, NoteId = noteId });
            await _likes.SaveChangesAsync();
            var count = await _likes.CountByNoteAsync(noteId);
            return (like == null, count);
        }

        public async Task<(bool Success, string? Error)> AddCommentAsync(AddCommentDto dto, string userId)
        {
            var comment = new Comment { NoteId = dto.NoteId, UserId = userId, Content = dto.Content, CreatedAt = DateTime.UtcNow };
            await _comments.AddAsync(comment);
            await _comments.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId, string userId, bool isAdmin)
        {
            var comment = await _comments.GetByIdAsync(commentId);
            if (comment == null) return (false, "Comment not found.");
            if (comment.UserId != userId && !isAdmin) return (false, "Access denied.");
            await _comments.DeleteAsync(comment);
            await _comments.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, double NewAverage, int Count)> RateNoteAsync(RateNoteDto dto, string userId)
        {
            var rating = await _ratings.GetUserRatingAsync(userId, dto.NoteId);
            if (rating != null) rating.Score = dto.Score;
            else await _ratings.AddAsync(new Rating { NoteId = dto.NoteId, UserId = userId, Score = dto.Score });
            await _ratings.SaveChangesAsync();
            var avg = await _ratings.GetAverageAsync(dto.NoteId);
            var count = await _ratings.GetCountAsync(dto.NoteId);
            return (true, avg, count);
        }

        public async Task<bool> ToggleFavoriteAsync(int noteId, string userId)
        {
            var fav = await _favorites.GetUserFavoriteAsync(userId, noteId);
            if (fav != null) await _favorites.DeleteAsync(fav);
            else await _favorites.AddAsync(new Favorite { UserId = userId, NoteId = noteId });
            await _favorites.SaveChangesAsync();
            return fav == null;
        }
    }

    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _users;
        private readonly INoteRepository _notes;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<User> _userManager;

        public ProfileService(IUserRepository users, INoteRepository notes, IWebHostEnvironment environment, UserManager<User> userManager)
        {
            _users = users;
            _notes = notes;
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var user = await _users.GetWithDetailsAsync(userId);
            if (user == null) return null;
            return new ProfileViewModel
            {
                Id = user.Id, FullName = user.FullName, Email = user.Email ?? "", Department = user.Department,
                ProfileImage = user.ProfileImage, CreatedAt = user.CreatedAt,
                UploadedNotes = (await _notes.GetByUserAsync(userId)).Select(n => new NoteCardViewModel { Id = n.Id, Title = n.Title, FileType = n.FileType, CourseName = n.Course?.CourseName ?? "" }).ToList()
            };
        }

        public async Task<(bool Success, string? Error)> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return (false, "User not found.");

            user.FullName = dto.FullName;
            user.Department = dto.Department;

            // 1. Password Change
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
                if (!result.Succeeded)
                {
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // 2. Profile Image Upload
            if (dto.ProfileImage != null)
            {
                var ext = Path.GetExtension(dto.ProfileImage.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (!allowed.Contains(ext)) return (false, "Invalid image format. Use JPG, PNG or WebP.");

                if (dto.ProfileImage.Length > 5 * 1024 * 1024) return (false, "File too large. Max 5MB.");

                var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                // Delete old image
                if (!string.IsNullOrEmpty(user.ProfileImage) && user.ProfileImage.StartsWith("/uploads/profiles/"))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, user.ProfileImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImage = $"/uploads/profiles/{fileName}";
            }

            // 3. Update Claims
            var claims = await _userManager.GetClaimsAsync(user);
            var fullNameClaim = claims.FirstOrDefault(c => c.Type == "FullName");
            if (fullNameClaim != null) await _userManager.ReplaceClaimAsync(user, fullNameClaim, new Claim("FullName", user.FullName));
            else await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));

            var profileImageClaim = claims.FirstOrDefault(c => c.Type == "ProfileImage");
            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                if (profileImageClaim != null) await _userManager.ReplaceClaimAsync(user, profileImageClaim, new Claim("ProfileImage", user.ProfileImage));
                else await _userManager.AddClaimAsync(user, new Claim("ProfileImage", user.ProfileImage));
            }

            await _users.UpdateAsync(user);
            await _users.SaveChangesAsync();
            return (true, null);
        }
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courses;
        public CourseService(ICourseRepository courses) => _courses = courses;
        public async Task<List<CourseSelectItem>> GetAllAsync() => (await _courses.GetAllAsync()).Select(c => new CourseSelectItem { Id = c.Id, CourseName = c.CourseName, Department = c.Department }).ToList();
        public async Task<List<string>> GetDepartmentsAsync() => await _courses.GetDepartmentsAsync();
        public async Task<List<CourseSelectItem>> SearchAsync(string? query, string? department) => (await _courses.SearchAsync(query, department)).Select(c => new CourseSelectItem { Id = c.Id, CourseName = c.CourseName, Department = c.Department }).ToList();
    }

    public class AdminService : IAdminService
    {
        private readonly IUserRepository _users;
        private readonly INoteRepository _notes;
        private readonly ICommentRepository _comments;
        public AdminService(IUserRepository users, INoteRepository notes, ICommentRepository comments) { _users = users; _notes = notes; _comments = comments; }

        public async Task<AdminDashboardViewModel> GetDashboardAsync()
        {
            var users = await _users.GetAllAsync();
            var notes = await _notes.GetAllAsync();
            var popularNotes = await _notes.GetPopularAsync(5);

            return new AdminDashboardViewModel 
            { 
                TotalUsers = users.Count, 
                TotalNotes = notes.Count(),
                TotalDownloads = notes.Sum(n => n.DownloadCount),
                TotalComments = (await _comments.GetAllAsync()).Count(),
                MostActiveUsers = new List<TopContributorViewModel>(), // Mock or implement if needed
                MonthlyUploads = new List<MonthlyStatViewModel>(), // Mock or implement if needed
                MostDownloadedNotes = popularNotes.Select(n => new NoteCardViewModel
                {
                    Id = n.Id, Title = n.Title, FileType = n.FileType,
                    DownloadCount = n.DownloadCount, LikeCount = n.Likes?.Count ?? 0,
                    UploaderName = n.User?.FullName ?? "", CourseName = n.Course?.CourseName ?? ""
                }).ToList()
            };
        }

        public async Task<List<AdminUserViewModel>> GetAllUsersAsync()
            => (await _users.GetAllAsync()).Select(u => new AdminUserViewModel { Id = u.Id, FullName = u.FullName, Email = u.Email ?? "", CreatedAt = u.CreatedAt }).ToList();

        public async Task<(bool Success, string? Error)> DeleteUserAsync(string userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return (false, "User not found.");
            await _users.DeleteAsync(user); await _users.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteNoteAsync(int noteId)
        {
            var note = await _notes.GetByIdAsync(noteId);
            if (note == null) return (false, "Note not found.");
            await _notes.DeleteAsync(note); await _notes.SaveChangesAsync();
            return (true, null);
        }

        public async Task<List<Comment>> GetAllCommentsAsync() => (await _comments.GetAllAsync()).ToList();
        public async Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId)
        {
            var comment = await _comments.GetByIdAsync(commentId);
            if (comment == null) return (false, "Comment not found.");
            await _comments.DeleteAsync(comment); await _comments.SaveChangesAsync();
            return (true, null);
        }
    }
}
