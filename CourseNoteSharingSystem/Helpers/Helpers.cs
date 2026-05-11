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
}
