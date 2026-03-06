namespace EduKidsGhana.Shared.Constants;

public static class AppConstants
{
    public const string AppName = "EduKids Ghana";
    public const string AppVersion = "1.0.0";
    public const string Tagline = "Learn, Listen, Play, and Grow";
    public const string DefaultLanguage = "en";
    public const string DefaultCurrency = "GHS";
    public const string CurrencySymbol = "GH₵";

    public static class Cache
    {
        public const int DefaultCacheMinutes = 30;
        public const int SubjectCacheMinutes = 60;
        public const int LessonCacheMinutes = 15;
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
    }

    public static class FileUpload
    {
        public const long MaxFileSizeBytes = 50 * 1024 * 1024;
        public static readonly string[] AllowedAudioExtensions = { ".mp3", ".wav", ".ogg" };
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    }
}
