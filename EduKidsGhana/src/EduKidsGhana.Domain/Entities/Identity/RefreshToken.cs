namespace EduKidsGhana.Domain.Entities.Identity;

public class RefreshToken : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
}
