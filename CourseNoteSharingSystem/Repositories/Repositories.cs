using CourseNoteSharingSystem.Data;
using CourseNoteSharingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseNoteSharingSystem.Repositories
{
    /// <summary>
    /// Generic base repository implementation.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;

        public Repository(AppDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _set.ToListAsync();
        public async Task AddAsync(T entity) => await _set.AddAsync(entity);
        public Task UpdateAsync(T entity) { _set.Update(entity); return Task.CompletedTask; }
        public Task DeleteAsync(T entity) { _set.Remove(entity); return Task.CompletedTask; }
        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }

    // ─── User Repository ───────────────────────────────────────────────────────
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db) { }

        public async Task<User?> GetByEmailAsync(string email)
            => await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetWithDetailsAsync(int id)
            => await _db.Users
                .Include(u => u.Notes).ThenInclude(n => n.Course)
                .Include(u => u.Likes).ThenInclude(l => l.Note).ThenInclude(n => n.User)
                .Include(u => u.Favorites).ThenInclude(f => f.Note).ThenInclude(n => n.Course)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<User>> GetTopContributorsAsync(int count)
            => await _db.Users
                .Include(u => u.Notes)
                .OrderByDescending(u => u.Notes.Count)
                .Take(count)
                .ToListAsync();
    }

    // ─── Note Repository ──────────────────────────────────────────────────────
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        public NoteRepository(AppDbContext db) : base(db) { }

        public async Task<Note?> GetWithDetailsAsync(int id)
            => await _db.Notes
                .Include(n => n.User)
                .Include(n => n.Course)
                .Include(n => n.Comments).ThenInclude(c => c.User)
                .Include(n => n.Likes)
                .Include(n => n.Ratings)
                .FirstOrDefaultAsync(n => n.Id == id);

        public async Task<List<Note>> GetRecentAsync(int count)
            => await _db.Notes
                .Include(n => n.User)
                .Include(n => n.Course)
                .Include(n => n.Likes)
                .Include(n => n.Ratings)
                .OrderByDescending(n => n.UploadDate)
                .Take(count)
                .ToListAsync();

        public async Task<List<Note>> GetPopularAsync(int count)
            => await _db.Notes
                .Include(n => n.User)
                .Include(n => n.Course)
                .Include(n => n.Likes)
                .Include(n => n.Ratings)
                .OrderByDescending(n => n.DownloadCount)
                .Take(count)
                .ToListAsync();

        public async Task<List<Note>> SearchAsync(string? query, int? courseId, string? department,
            string? fileType, string sortBy, int page, int pageSize)
        {
            var q = BuildQuery(query, courseId, department, fileType);
            q = sortBy switch
            {
                "mostliked" => q.OrderByDescending(n => n.Likes.Count),
                "mostdownloaded" => q.OrderByDescending(n => n.DownloadCount),
                _ => q.OrderByDescending(n => n.UploadDate)
            };
            return await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> CountSearchAsync(string? query, int? courseId, string? department, string? fileType)
            => await BuildQuery(query, courseId, department, fileType).CountAsync();

        private IQueryable<Note> BuildQuery(string? query, int? courseId, string? department, string? fileType)
        {
            var q = _db.Notes
                .Include(n => n.User)
                .Include(n => n.Course)
                .Include(n => n.Likes)
                .Include(n => n.Ratings)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(n => n.Title.ToLower().Contains(query.ToLower()) ||
                                 n.User.FullName.ToLower().Contains(query.ToLower()));
            if (courseId.HasValue)
                q = q.Where(n => n.CourseId == courseId.Value);
            if (!string.IsNullOrWhiteSpace(department))
                q = q.Where(n => n.Course.Department == department);
            if (!string.IsNullOrWhiteSpace(fileType))
                q = q.Where(n => n.FileType == fileType);

            return q;
        }

        public async Task<List<Note>> GetByUserAsync(int userId)
            => await _db.Notes
                .Include(n => n.Course)
                .Include(n => n.Likes)
                .Include(n => n.Ratings)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.UploadDate)
                .ToListAsync();

        public async Task<List<Note>> GetLikedByUserAsync(int userId)
            => await _db.Likes
                .Where(l => l.UserId == userId)
                .Include(l => l.Note).ThenInclude(n => n.Course)
                .Include(l => l.Note).ThenInclude(n => n.User)
                .Include(l => l.Note).ThenInclude(n => n.Likes)
                .Include(l => l.Note).ThenInclude(n => n.Ratings)
                .Select(l => l.Note)
                .ToListAsync();

        public async Task<List<Note>> GetFavoritedByUserAsync(int userId)
            => await _db.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Note).ThenInclude(n => n.Course)
                .Include(f => f.Note).ThenInclude(n => n.User)
                .Include(f => f.Note).ThenInclude(n => n.Likes)
                .Include(f => f.Note).ThenInclude(n => n.Ratings)
                .Select(f => f.Note)
                .ToListAsync();

        public async Task IncrementDownloadCountAsync(int noteId)
        {
            var note = await _set.FindAsync(noteId);
            if (note != null)
            {
                note.DownloadCount++;
                await _db.SaveChangesAsync();
            }
        }
    }

    // ─── Course Repository ───────────────────────────────────────────────────
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext db) : base(db) { }

        public async Task<List<string>> GetDepartmentsAsync()
            => await _db.Courses.Select(c => c.Department).Distinct().OrderBy(d => d).ToListAsync();

        public async Task<List<Course>> SearchAsync(string? query, string? department)
        {
            var q = _db.Courses.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(c => c.CourseName.ToLower().Contains(query.ToLower()));
            if (!string.IsNullOrWhiteSpace(department))
                q = q.Where(c => c.Department == department);
            return await q.OrderBy(c => c.CourseName).ToListAsync();
        }
    }

    // ─── Comment Repository ──────────────────────────────────────────────────
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext db) : base(db) { }

        public async Task<List<Comment>> GetByNoteAsync(int noteId)
            => await _db.Comments
                .Include(c => c.User)
                .Where(c => c.NoteId == noteId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
    }

    // ─── Like Repository ─────────────────────────────────────────────────────
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        public LikeRepository(AppDbContext db) : base(db) { }

        public async Task<Like?> GetUserLikeAsync(int userId, int noteId)
            => await _db.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.NoteId == noteId);

        public async Task<int> CountByNoteAsync(int noteId)
            => await _db.Likes.CountAsync(l => l.NoteId == noteId);
    }

    // ─── Rating Repository ───────────────────────────────────────────────────
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(AppDbContext db) : base(db) { }

        public async Task<Rating?> GetUserRatingAsync(int userId, int noteId)
            => await _db.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.NoteId == noteId);

        public async Task<double> GetAverageAsync(int noteId)
        {
            var ratings = await _db.Ratings.Where(r => r.NoteId == noteId).ToListAsync();
            return ratings.Count == 0 ? 0 : ratings.Average(r => r.Score);
        }

        public async Task<int> GetCountAsync(int noteId)
            => await _db.Ratings.CountAsync(r => r.NoteId == noteId);
    }

    // ─── Favorite Repository ─────────────────────────────────────────────────
    public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(AppDbContext db) : base(db) { }

        public async Task<Favorite?> GetUserFavoriteAsync(int userId, int noteId)
            => await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.NoteId == noteId);
    }
}
