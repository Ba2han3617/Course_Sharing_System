using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Models;
using CourseNoteSharingSystem.ViewModels;

namespace CourseNoteSharingSystem.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? Error)> RegisterAsync(RegisterDto dto);
        Task<User?> LoginAsync(LoginDto dto);
    }

    public interface INoteService
    {
        Task<NoteDetailViewModel?> GetNoteDetailAsync(int noteId, int? currentUserId);
        Task<List<NoteCardViewModel>> GetRecentNotesAsync(int count);
        Task<List<NoteCardViewModel>> GetPopularNotesAsync(int count);
        Task<NoteSearchViewModel> SearchAsync(NoteSearchViewModel filter);
        Task<(bool Success, string? Error, int NoteId)> UploadNoteAsync(UploadNoteDto dto, int userId);
        Task<(bool Success, string? Error)> EditNoteAsync(EditNoteDto dto, int userId);
        Task<(bool Success, string? Error)> DeleteNoteAsync(int noteId, int userId, bool isAdmin);
        Task<(bool Success, string? FilePath, string? FileName, string? ContentType)> GetDownloadAsync(int noteId);
    }

    public interface IInteractionService
    {
        Task<(bool IsLiked, int LikeCount)> ToggleLikeAsync(int noteId, int userId);
        Task<(bool Success, string? Error)> AddCommentAsync(AddCommentDto dto, int userId);
        Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId, int userId, bool isAdmin);
        Task<(bool Success, double NewAverage, int Count)> RateNoteAsync(RateNoteDto dto, int userId);
        Task<bool> ToggleFavoriteAsync(int noteId, int userId);
    }

    public interface IProfileService
    {
        Task<ProfileViewModel?> GetProfileAsync(int userId);
        Task<(bool Success, string? Error)> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    }

    public interface ICourseService
    {
        Task<List<CourseSelectItem>> GetAllAsync();
        Task<List<string>> GetDepartmentsAsync();
        Task<List<CourseSelectItem>> SearchAsync(string? query, string? department);
    }

    public interface IAdminService
    {
        Task<AdminDashboardViewModel> GetDashboardAsync();
        Task<List<User>> GetAllUsersAsync();
        Task<(bool Success, string? Error)> DeleteUserAsync(int userId);
        Task<(bool Success, string? Error)> DeleteNoteAsync(int noteId);
        Task<List<Models.Comment>> GetAllCommentsAsync();
        Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId);
    }
}
