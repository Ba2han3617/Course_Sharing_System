using CourseNoteSharingSystem.Repositories;
using CourseNoteSharingSystem.Services;
using CourseNoteSharingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    /// <summary>
    /// Home controller – public, no auth required.
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly INoteService _noteService;
        private readonly ICourseService _courseService;
        private readonly IUserRepository _userRepo;

        public HomeController(INoteService noteService, ICourseService courseService, IUserRepository userRepo)
        {
            _noteService = noteService;
            _courseService = courseService;
            _userRepo = userRepo;
        }

        public async Task<IActionResult> Index()
        {
            var popular = await _noteService.GetPopularNotesAsync(6);
            var recent = await _noteService.GetRecentNotesAsync(6);
            var topUsers = await _userRepo.GetTopContributorsAsync(5);
            var departments = await _courseService.GetDepartmentsAsync();

            var vm = new HomeViewModel
            {
                PopularNotes = popular,
                RecentNotes = recent,
                Departments = departments,
                TopContributors = topUsers.Select(u => new TopContributorViewModel
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    ProfileImage = u.ProfileImage,
                    Department = u.Department,
                    NoteCount = u.Notes?.Count ?? 0
                }).ToList()
            };

            return View(vm);
        }

        public IActionResult About() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
