using CourseNoteSharingSystem.DTOs;
using CourseNoteSharingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseNoteSharingSystem.Controllers
{
    /// <summary>
    /// Handles AJAX-based interactions: likes, comments, ratings, favorites.
    /// All endpoints return JSON for AJAX consumption.
    /// </summary>
    public class InteractionController : BaseController
    {
        private readonly IInteractionService _interaction;

        public InteractionController(IInteractionService interaction) => _interaction = interaction;

        // POST /Interaction/ToggleLike
        [HttpPost]
        public async Task<IActionResult> ToggleLike([FromBody] int noteId)
        {
            if (!IsAuthenticated) return Unauthorized(new { error = "Login required." });
            var (isLiked, count) = await _interaction.ToggleLikeAsync(noteId, CurrentUserId!.Value);
            return Json(new { isLiked, count });
        }

        // POST /Interaction/AddComment
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto dto)
        {
            if (!IsAuthenticated) return Unauthorized(new { error = "Login required." });
            if (!ModelState.IsValid) return BadRequest(new { error = "Invalid data." });

            var (success, error) = await _interaction.AddCommentAsync(dto, CurrentUserId!.Value);
            if (!success) return BadRequest(new { error });

            return Json(new
            {
                success = true,
                userName = (string)ViewBag.UserName ?? "",
                userAvatar = (string?)ViewBag.UserAvatar,
                content = dto.Content,
                createdAt = DateTime.UtcNow.ToString("MMM dd, yyyy")
            });
        }

        // POST /Interaction/DeleteComment
        [HttpPost]
        public async Task<IActionResult> DeleteComment([FromBody] int commentId)
        {
            if (!IsAuthenticated) return Unauthorized(new { error = "Login required." });
            var (success, error) = await _interaction.DeleteCommentAsync(commentId, CurrentUserId!.Value, IsAdmin);
            if (!success) return BadRequest(new { error });
            return Json(new { success = true });
        }

        // POST /Interaction/Rate
        [HttpPost]
        public async Task<IActionResult> Rate([FromBody] RateNoteDto dto)
        {
            if (!IsAuthenticated) return Unauthorized(new { error = "Login required." });
            if (!ModelState.IsValid) return BadRequest(new { error = "Invalid score." });

            var (success, avg, count) = await _interaction.RateNoteAsync(dto, CurrentUserId!.Value);
            return Json(new { success, avg, count });
        }

        // POST /Interaction/ToggleFavorite
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite([FromBody] int noteId)
        {
            if (!IsAuthenticated) return Unauthorized(new { error = "Login required." });
            var isFavorited = await _interaction.ToggleFavoriteAsync(noteId, CurrentUserId!.Value);
            return Json(new { isFavorited });
        }
    }
}
