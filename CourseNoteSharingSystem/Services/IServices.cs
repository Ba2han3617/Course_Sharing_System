using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Models;
using CourseNoteSharingSystem.ViewModels;

namespace CourseNoteSharingSystem.Services
{
    public interface INoteService
    {
        Task<NoteDetailViewModel?> GetNoteDetailAsync(int noteId, string? currentUserId);
        Task<List<NoteCardViewModel>> GetRecentNotesAsync(int count);
        Task<List<NoteCardViewModel>> GetPopularNotesAsync(int count);
        Task<NoteSearchViewModel> SearchAsync(NoteSearchViewModel filter);
        Task<(bool Success, string? Error, int NoteId)> UploadNoteAsync(UploadNoteDto dto, string userId);
        Task<(bool Success, string? Error)> EditNoteAsync(EditNoteDto dto, string userId);
        Task<(bool Success, string? Error)> DeleteNoteAsync(int noteId, string userId, bool isAdmin);
        Task<(bool Success, string? FilePath, string? FileName, string? ContentType)> GetDownloadAsync(int noteId);
    }

    public interface IInteractionService
    {
        Task<(bool IsLiked, int LikeCount)> ToggleLikeAsync(int noteId, string userId);
        Task<(bool Success, string? Error)> AddCommentAsync(AddCommentDto dto, string userId);
        Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId, string userId, bool isAdmin);
        Task<(bool Success, double NewAverage, int Count)> RateNoteAsync(RateNoteDto dto, string userId);
        Task<bool> ToggleFavoriteAsync(int noteId, string userId);
    }

    public interface IProfileService
    {
        Task<ProfileViewModel?> GetProfileAsync(string userId);
        Task<(bool Success, string? Error)> UpdateProfileAsync(string userId, UpdateProfileDto dto);
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
        Task<List<AdminUserViewModel>> GetAllUsersAsync();
        Task<(bool Success, string? Error)> DeleteUserAsync(string userId);
        Task<(bool Success, string? Error)> DeleteNoteAsync(int noteId);
        Task<List<Models.Comment>> GetAllCommentsAsync();
        Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId);
    }
}
