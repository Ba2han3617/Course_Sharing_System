using System.Security.Claims;
using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Models;
using CourseNoteSharingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    public class AuthController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ─── USER LOGIN ──────────────────────────────────────────────────────────
        [HttpGet("/login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            if (IsAuthenticated) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl)
        {
            if (!ModelState.IsValid) return View(dto);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(dto);
            }

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(dto);
            }

            // KESİN KONTROL: Admin kullanıcıları normal login'den GİREMEZ
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                await _signInManager.SignOutAsync(); // Yanlışlıkla girmişse oturumu kapat
                ModelState.AddModelError("", "Admin hesapları bu ekranı kullanamaz. Lütfen admin panelinden giriş yapın.");
                return View(dto);
            }

            // After login, redirect based on role if no returnUrl
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/admnistrative")]
        [AllowAnonymous]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost("/admnistrative")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Hatalı kimlik bilgileri.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Hatalı kimlik bilgileri.");
                return View(model);
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                await _signInManager.SignOutAsync();

                ModelState.AddModelError("",
                "Bu hesap admin yetkisine sahip değil.");

                return View(model);
            }

            return RedirectToAction("Dashboard", "Admin");
        }

        // ─── LOGOUT & OTHERS ─────────────────────────────────────────────────────
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("Auth/AccessDenied")]
        public IActionResult AccessDenied() => View();

        [HttpGet]
        public IActionResult Register()
        {
            if (IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return View(dto);
        }
    }
}
