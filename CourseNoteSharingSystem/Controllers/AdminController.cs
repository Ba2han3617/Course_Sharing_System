using CourseNoteSharingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    /// <summary>
    /// Admin panel – accessible only to users with Admin role.
    /// The middleware enforces this check before reaching here.
    /// </summary>
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService) => _adminService = adminService;

        private IActionResult CheckAdmin()
        {
            if (!IsAdmin) return Forbid();
            return Ok();
        }

        // GET /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin) return Forbid();
            var vm = await _adminService.GetDashboardAsync();
            return View(vm);
        }

        // GET /Admin/Users
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin) return Forbid();
            var users = await _adminService.GetAllUsersAsync();
            return View(users);
        }

        // POST /Admin/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin) return Forbid();
            var (success, error) = await _adminService.DeleteUserAsync(id);
            if (success) ShowSuccess("User deleted successfully.");
            else ShowError(error!);
            return RedirectToAction(nameof(Users));
        }

        // GET /Admin/Notes
        public async Task<IActionResult> Notes()
        {
            if (!IsAdmin) return Forbid();
            // Reuse search with no filter to get all notes
            var notes = await _adminService.GetDashboardAsync();
            return View(notes.MostDownloadedNotes);
        }

        // POST /Admin/DeleteNote
        [HttpPost]
        public async Task<IActionResult> DeleteNote(int id)
        {
            if (!IsAdmin) return Forbid();
            var (success, error) = await _adminService.DeleteNoteAsync(id);
            if (success) ShowSuccess("Note removed successfully.");
            else ShowError(error!);
            return RedirectToAction(nameof(Notes));
        }

        // GET /Admin/Comments
        public async Task<IActionResult> Comments()
        {
            if (!IsAdmin) return Forbid();
            var comments = await _adminService.GetAllCommentsAsync();
            return View(comments);
        }

        // POST /Admin/DeleteComment
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (!IsAdmin) return Forbid();
            var (success, error) = await _adminService.DeleteCommentAsync(id);
            if (success) ShowSuccess("Comment deleted.");
            else ShowError(error!);
            return RedirectToAction(nameof(Comments));
        }
    }
}
