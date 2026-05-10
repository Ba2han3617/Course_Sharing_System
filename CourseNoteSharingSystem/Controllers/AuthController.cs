using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Helpers;
using CourseNoteSharingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    /// <summary>
    /// Handles login, registration, and logout.
    /// </summary>
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        // GET /Auth/Login
        public IActionResult Login(string? returnUrl)
        {
            if (IsAuthenticated) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl)
        {
            if (!ModelState.IsValid) return View(dto);

            var user = await _authService.LoginAsync(dto);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(dto);
            }

            SessionHelper.SetUser(HttpContext.Session, user);
            ShowSuccess($"Welcome back, {user.FullName}!");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return user.Role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Index", "Home");
        }

        // GET /Auth/Register
        public IActionResult Register()
        {
            if (IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        // POST /Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var (success, error) = await _authService.RegisterAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(dto);
            }

            ShowSuccess("Account created! Please log in.");
            return RedirectToAction(nameof(Login));
        }

        // GET /Auth/Logout
        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            ShowSuccess("You have been logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET /Auth/ForgotPassword
        public IActionResult ForgotPassword() => View();
    }
}
