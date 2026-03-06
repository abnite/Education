namespace EduKidsGhana.Domain.Entities.System;

public class MediaFile : BaseAuditableEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; } = 0;
    public string? AltText { get; set; }
    public string? Category { get; set; }
    public bool IsPublic { get; set; } = true;
}
