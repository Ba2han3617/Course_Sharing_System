using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseNoteSharingSystem.Controllers
{
    /// <summary>
    /// Base controller that injects Identity-based user info into ViewBag for all views.
    /// Uses ClaimsPrincipal from ASP.NET Core Identity (no session-based auth).
    /// </summary>
    public abstract class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.UserId = CurrentUserId;
            ViewBag.UserName = User.FindFirstValue("FullName") ?? User.Identity?.Name;
            ViewBag.UserRole = User.IsInRole("Admin") ? "Admin" : "User";
            ViewBag.UserAvatar = User.FindFirstValue("ProfileImage");
            ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
            ViewBag.IsAdmin = User.IsInRole("Admin");
        }

        protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        protected bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;
        protected bool IsAdmin => User.IsInRole("Admin");

        protected void ShowSuccess(string msg) => TempData["SuccessMessage"] = msg;
        protected void ShowError(string msg) => TempData["ErrorMessage"] = msg;
    }
}
