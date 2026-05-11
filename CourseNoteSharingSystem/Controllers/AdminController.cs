using CourseNoteSharingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService) => _adminService = adminService;
        
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        public async Task<IActionResult> Dashboard()
        {
            var vm = await _adminService.GetDashboardAsync();
            return View(vm);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var (success, error) = await _adminService.DeleteUserAsync(id);
            if (success) ShowSuccess("User deleted successfully.");
            else ShowError(error!);
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Notes()
        {
            var stats = await _adminService.GetDashboardAsync();
            return View(stats.MostDownloadedNotes);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var (success, error) = await _adminService.DeleteNoteAsync(id);
            if (success) ShowSuccess("Note removed.");
            else ShowError(error!);
            return RedirectToAction(nameof(Notes));
        }

        public async Task<IActionResult> Comments()
        {
            var comments = await _adminService.GetAllCommentsAsync();
            return View(comments);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var (success, error) = await _adminService.DeleteCommentAsync(id);
            if (success) ShowSuccess("Comment deleted.");
            else ShowError(error!);
            return RedirectToAction(nameof(Comments));
        }
    }
}
