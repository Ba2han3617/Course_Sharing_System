using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Models;
using CourseNoteSharingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ProfileController(IProfileService profileService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _profileService = profileService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET /Profile – current user profile
        public async Task<IActionResult> Index()
        {
            var vm = await _profileService.GetProfileAsync(CurrentUserId!);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET /Profile/View/5 – view another user's public profile
        [AllowAnonymous]
        [ActionName("View")]
        public async Task<IActionResult> PublicProfile(string id)
        {
            var vm = await _profileService.GetProfileAsync(id);
            if (vm == null) return NotFound();
            return View("Index", vm);
        }

        // GET /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _profileService.GetProfileAsync(CurrentUserId!);
            if (user == null) return NotFound();

            var dto = new UpdateProfileDto
            {
                FullName = user.FullName,
                Department = user.Department
            };
            return View(dto);
        }

        // POST /Profile/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var (success, error) = await _profileService.UpdateProfileAsync(CurrentUserId!, dto);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(dto);
            }

            // Refresh the sign-in cookie to update claims (e.g. FullName)
            var user = await _userManager.FindByIdAsync(CurrentUserId!);
            if (user != null)
            {
                await _signInManager.RefreshSignInAsync(user);
            }

            ShowSuccess("Profile updated successfully!");
            return RedirectToAction(nameof(Index));
        }
    }
}
