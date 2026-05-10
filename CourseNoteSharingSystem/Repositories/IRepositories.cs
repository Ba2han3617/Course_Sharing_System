using CourseNoteSharingSystem.Models;

namespace CourseNoteSharingSystem.Repositories
{
    /// <summary>
    /// Generic repository interface for common CRUD operations.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetWithDetailsAsync(int id);
        Task<List<User>> GetTopContributorsAsync(int count);
    }

    public interface INoteRepository : IRepository<Note>
    {
        Task<Note?> GetWithDetailsAsync(int id);
        Task<List<Note>> GetRecentAsync(int count);
        Task<List<Note>> GetPopularAsync(int count);
        Task<List<Note>> SearchAsync(string? query, int? courseId, string? department, string? fileType, string sortBy, int page, int pageSize);
        Task<int> CountSearchAsync(string? query, int? courseId, string? department, string? fileType);
        Task<List<Note>> GetByUserAsync(int userId);
        Task<List<Note>> GetLikedByUserAsync(int userId);
        Task<List<Note>> GetFavoritedByUserAsync(int userId);
        Task IncrementDownloadCountAsync(int noteId);
    }

    public interface ICourseRepository : IRepository<Course>
    {
        Task<List<string>> GetDepartmentsAsync();
        Task<List<Course>> SearchAsync(string? query, string? department);
    }

    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<Comment>> GetByNoteAsync(int noteId);
    }

    public interface ILikeRepository : IRepository<Like>
    {
        Task<Like?> GetUserLikeAsync(int userId, int noteId);
        Task<int> CountByNoteAsync(int noteId);
    }

    public interface IRatingRepository : IRepository<Rating>
    {
        Task<Rating?> GetUserRatingAsync(int userId, int noteId);
        Task<double> GetAverageAsync(int noteId);
        Task<int> GetCountAsync(int noteId);
    }

    public interface IFavoriteRepository : IRepository<Favorite>
    {
        Task<Favorite?> GetUserFavoriteAsync(int userId, int noteId);
    }
}
