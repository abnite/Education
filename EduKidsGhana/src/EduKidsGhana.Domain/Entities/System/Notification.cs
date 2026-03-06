namespace EduKidsGhana.Domain.Entities.System;

public class Notification : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string NotificationType { get; set; } = "Info";
}
