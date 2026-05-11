using CourseNoteSharingSystem.Models;
using CourseNoteSharingSystem.Repositories;
using CourseNoteSharingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseNoteSharingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Courses/[action]")]
    public class CourseController : BaseController
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // GET: /Admin/Courses/Index
        [Route("/Admin/Courses")]
        [Route("/Admin/Courses/Index")]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllAsync();
            var viewModel = courses.Select(c => new CourseListViewModel
            {
                Id = c.Id,
                CourseName = c.CourseName,
                Code = c.Code,
                Department = c.Department,
                Semester = c.Semester,
                CreatedAt = c.CreatedAt
            }).OrderByDescending(c => c.CreatedAt).ToList();

            return View(viewModel);
        }

        // GET: /Admin/Courses/Create
        public IActionResult Create()
        {
            return View(new CreateCourseViewModel());
        }

        // POST: /Admin/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Check duplicate code
            var all = await _courseRepository.GetAllAsync();
            if (all.Any(c => c.Code.Equals(model.Code, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Code", "A course with this code already exists.");
                return View(model);
            }

            var course = new Course
            {
                CourseName = model.CourseName,
                Code = model.Code,
                Description = model.Description,
                Department = model.Department,
                Semester = model.Semester,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Courses/Edit/5
        [Route("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return NotFound();

            var viewModel = new EditCourseViewModel
            {
                Id = course.Id,
                CourseName = course.CourseName,
                Code = course.Code,
                Description = course.Description,
                Department = course.Department,
                Semester = course.Semester
            };

            return View(viewModel);
        }

        // POST: /Admin/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id}")]
        public async Task<IActionResult> Edit(int id, EditCourseViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return NotFound();

            // Check duplicate code (excluding current course)
            var all = await _courseRepository.GetAllAsync();
            if (all.Any(c => c.Id != id && c.Code.Equals(model.Code, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Code", "Another course with this code already exists.");
                return View(model);
            }

            course.CourseName = model.CourseName;
            course.Code = model.Code;
            course.Description = model.Description;
            course.Department = model.Department;
            course.Semester = model.Semester;
            course.UpdatedAt = DateTime.UtcNow;

            await _courseRepository.UpdateAsync(course);
            await _courseRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Courses/Details/5
        [Route("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return NotFound();

            return View(course);
        }

        // POST: /Admin/Courses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return NotFound();

            await _courseRepository.DeleteAsync(course);
            await _courseRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
