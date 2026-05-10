using CourseNoteSharingSystem.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseNoteSharingSystem.Controllers
{
    /// <summary>
    /// Base controller that injects session-based user info into ViewBag for all views.
    /// </summary>
    public abstract class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.UserId = SessionHelper.GetUserId(HttpContext.Session);
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            ViewBag.UserRole = SessionHelper.GetUserRole(HttpContext.Session);
            ViewBag.UserAvatar = SessionHelper.GetUserAvatar(HttpContext.Session);
            ViewBag.IsAuthenticated = SessionHelper.IsAuthenticated(HttpContext.Session);
            ViewBag.IsAdmin = SessionHelper.IsAdmin(HttpContext.Session);
        }

        protected int? CurrentUserId => SessionHelper.GetUserId(HttpContext.Session);
        protected bool IsAuthenticated => SessionHelper.IsAuthenticated(HttpContext.Session);
        protected bool IsAdmin => SessionHelper.IsAdmin(HttpContext.Session);

        protected IActionResult RequireLogin(string? returnUrl = null)
        {
            if (!IsAuthenticated)
            {
                var url = returnUrl ?? Request.Path.Value ?? "/";
                return Redirect($"/Auth/Login?returnUrl={Uri.EscapeDataString(url)}");
            }
            return Ok(); // won't be used
        }

        protected void ShowSuccess(string msg) => TempData["SuccessMessage"] = msg;
        protected void ShowError(string msg) => TempData["ErrorMessage"] = msg;
    }
}
