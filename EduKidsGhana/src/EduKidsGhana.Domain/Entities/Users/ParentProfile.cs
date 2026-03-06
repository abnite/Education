using EduKidsGhana.Domain.Entities.Identity;

namespace EduKidsGhana.Domain.Entities.Users;

public class ParentProfile : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Region { get; set; }
    public string? City { get; set; }
    public bool ReceiveProgressEmails { get; set; } = true;
    public bool ReceiveWeeklyReports { get; set; } = true;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<ParentLearnerLink> LearnerLinks { get; set; } = new List<ParentLearnerLink>();
}
