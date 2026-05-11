using CourseNoteSharingSystem.Services;
using CourseNoteSharingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    public class CoursesController : BaseController
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService) => _courseService = courseService;

        // GET /Courses
        public async Task<IActionResult> Index(string? query, string? department)
        {
            var courses = await _courseService.SearchAsync(query, department);
            var departments = await _courseService.GetDepartmentsAsync();
            ViewBag.Departments = departments;
            ViewBag.Query = query;
            ViewBag.SelectedDepartment = department;
            return View(courses);
        }

        // GET /Courses/Search – AJAX autocomplete
        [HttpGet]
        public async Task<IActionResult> Search(string? q)
        {
            var courses = await _courseService.SearchAsync(q, null);
            return Json(courses.Select(c => new { c.Id, c.CourseName, c.Department }));
        }
    }
}
