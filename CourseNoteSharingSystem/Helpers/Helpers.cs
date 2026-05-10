namespace CourseNoteSharingSystem.Helpers
{
    /// <summary>
    /// Extension methods and helper utilities used across the application.
    /// </summary>
    public static class FileHelper
    {
        private static readonly Dictionary<string, string> IconMap = new()
        {
            { "pdf",  "bi-file-earmark-pdf-fill text-danger" },
            { "docx", "bi-file-earmark-word-fill text-primary" },
            { "pptx", "bi-file-earmark-slides-fill text-warning" },
            { "zip",  "bi-file-earmark-zip-fill text-success" }
        };

        public static string GetIcon(string fileType)
            => IconMap.TryGetValue(fileType.ToLower(), out var icon) ? icon : "bi-file-earmark text-secondary";

        public static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
        }
    }

    public static class SessionHelper
    {
        public const string UserIdKey = "UserId";
        public const string UserEmailKey = "UserEmail";
        public const string UserNameKey = "UserName";
        public const string UserRoleKey = "UserRole";
        public const string UserAvatarKey = "UserAvatar";

        public static void SetUser(ISession session, Models.User user)
        {
            session.SetInt32(UserIdKey, user.Id);
            session.SetString(UserEmailKey, user.Email);
            session.SetString(UserNameKey, user.FullName);
            session.SetString(UserRoleKey, user.Role);
            if (user.ProfileImage != null)
                session.SetString(UserAvatarKey, user.ProfileImage);
        }

        public static void Clear(ISession session) => session.Clear();

        public static int? GetUserId(ISession session) => session.GetInt32(UserIdKey);
        public static string? GetUserName(ISession session) => session.GetString(UserNameKey);
        public static string? GetUserRole(ISession session) => session.GetString(UserRoleKey);
        public static string? GetUserAvatar(ISession session) => session.GetString(UserAvatarKey);
        public static bool IsAuthenticated(ISession session) => session.GetInt32(UserIdKey).HasValue;
        public static bool IsAdmin(ISession session) => session.GetString(UserRoleKey) == "Admin";
    }
}
