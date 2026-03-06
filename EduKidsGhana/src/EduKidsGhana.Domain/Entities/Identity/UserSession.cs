namespace EduKidsGhana.Domain.Entities.Identity;

public class UserSession : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string SessionToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ApplicationUser User { get; set; } = null!;
}
