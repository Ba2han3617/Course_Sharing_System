using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Services;
using CourseNoteSharingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    /// <summary>
    /// Handles note CRUD, download, and search operations.
    /// </summary>
    public class NotesController : BaseController
    {
        private readonly INoteService _noteService;
        private readonly ICourseService _courseService;

        public NotesController(INoteService noteService, ICourseService courseService)
        {
            _noteService = noteService;
            _courseService = courseService;
        }

        // GET /Notes – search/browse page
        public async Task<IActionResult> Index(NoteSearchViewModel filter)
        {
            filter.PageSize = 12;
            var vm = await _noteService.SearchAsync(filter);
            return View(vm);
        }

        // GET /Notes/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var vm = await _noteService.GetNoteDetailAsync(id, CurrentUserId);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET /Notes/Upload
        [Authorize]
        public async Task<IActionResult> Upload()
        {
            ViewBag.Courses = await _courseService.GetAllAsync();
            return View();
        }

        // POST /Notes/Upload
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload(UploadNoteDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = await _courseService.GetAllAsync();
                return View(dto);
            }

            var (success, error, noteId) = await _noteService.UploadNoteAsync(dto, CurrentUserId!);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                ViewBag.Courses = await _courseService.GetAllAsync();
                return View(dto);
            }

            ShowSuccess("Note uploaded successfully!");
            return RedirectToAction(nameof(Detail), new { id = noteId });
        }

        // GET /Notes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _noteService.GetNoteDetailAsync(id, CurrentUserId);
            if (note == null) return NotFound();
            if (note.UploaderId != CurrentUserId && !IsAdmin) return Forbid();

            ViewBag.Courses = await _courseService.GetAllAsync();
            var dto = new EditNoteDto { Id = note.Id, Title = note.Title, Description = note.Description, CourseId = note.CourseId };
            return View(dto);
        }

        // POST /Notes/Edit/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditNoteDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = await _courseService.GetAllAsync();
                return View(dto);
            }

            var (success, error) = await _noteService.EditNoteAsync(dto, CurrentUserId!);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                ViewBag.Courses = await _courseService.GetAllAsync();
                return View(dto);
            }

            ShowSuccess("Note updated successfully!");
            return RedirectToAction(nameof(Detail), new { id = dto.Id });
        }

        // POST /Notes/Delete/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _noteService.DeleteNoteAsync(id, CurrentUserId!, IsAdmin);
            if (success) ShowSuccess("Note deleted successfully.");
            else ShowError(error!);
            return RedirectToAction("Index", "Home");
        }

        // GET /Notes/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var (success, filePath, fileName, contentType) = await _noteService.GetDownloadAsync(id);
            if (!success) return NotFound();
            return PhysicalFile(filePath!, contentType!, fileName);
        }

        // GET /Notes/Search – AJAX live search endpoint
        [HttpGet]
        public async Task<IActionResult> Search(string? q)
        {
            var filter = new NoteSearchViewModel { SearchQuery = q, PageSize = 6 };
            var vm = await _noteService.SearchAsync(filter);
            return Json(vm.Notes.Select(n => new { n.Id, n.Title, n.CourseName, n.FileType, n.UploaderName }));
        }
    }
}
