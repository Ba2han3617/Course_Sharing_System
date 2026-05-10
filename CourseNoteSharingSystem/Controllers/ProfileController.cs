using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService) => _profileService = profileService;

        // GET /Profile – current user profile
        public async Task<IActionResult> Index()
        {
            if (!IsAuthenticated) return Redirect("/Auth/Login");
            var vm = await _profileService.GetProfileAsync(CurrentUserId!.Value);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET /Profile/View/5 – view another user's public profile
        public async Task<IActionResult> View(int id)
        {
            var vm = await _profileService.GetProfileAsync(id);
            if (vm == null) return NotFound();
            return View("Index", vm);
        }

        // GET /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            if (!IsAuthenticated) return Redirect("/Auth/Login");
            var user = await _profileService.GetProfileAsync(CurrentUserId!.Value);
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
            if (!IsAuthenticated) return Redirect("/Auth/Login");
            if (!ModelState.IsValid) return View(dto);

            var (success, error) = await _profileService.UpdateProfileAsync(CurrentUserId!.Value, dto);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(dto);
            }

            // Refresh session name
            var user = await _profileService.GetProfileAsync(CurrentUserId!.Value);
            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.FullName);
                if (user.ProfileImage != null)
                    HttpContext.Session.SetString("UserAvatar", user.ProfileImage);
            }

            ShowSuccess("Profile updated successfully!");
            return RedirectToAction(nameof(Index));
        }
    }
}
