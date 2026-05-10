using CourseNoteSharingSystem.Helpers;

namespace CourseNoteSharingSystem.Middleware
{
    /// <summary>
    /// Middleware that protects routes requiring authentication.
    /// Pages decorated with [RequireAuth] attribute pattern are handled here.
    /// </summary>
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        // Paths accessible without login
        private static readonly string[] PublicPaths =
        [
            "/", "/Home", "/Home/Index", "/Home/About",
            "/Auth/Login", "/Auth/Register", "/Auth/ForgotPassword",
            "/Notes/Index", "/Notes/Detail", "/Notes/Search",
            "/Courses",
        ];

        public AuthMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "/";

            // Allow static files, API calls, and public pages
            if (path.StartsWith("/lib") || path.StartsWith("/css") || path.StartsWith("/js") ||
                path.StartsWith("/uploads") || path.StartsWith("/avatars") || path.StartsWith("/img"))
            {
                await _next(context);
                return;
            }

            // Admin protection
            if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (!SessionHelper.IsAuthenticated(context.Session) || !SessionHelper.IsAdmin(context.Session))
                {
                    context.Response.Redirect("/Auth/Login?returnUrl=" + Uri.EscapeDataString(path));
                    return;
                }
            }

            // Routes requiring any authentication
            if (path.StartsWith("/Profile", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/Notes/Upload", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/Notes/Edit", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/Notes/Delete", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/Interaction", StringComparison.OrdinalIgnoreCase))
            {
                if (!SessionHelper.IsAuthenticated(context.Session))
                {
                    context.Response.Redirect("/Auth/Login?returnUrl=" + Uri.EscapeDataString(path));
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthGuard(this IApplicationBuilder app)
            => app.UseMiddleware<AuthMiddleware>();
    }
}
