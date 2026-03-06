using EduKidsGhana.Domain.Entities.Users;
using EduKidsGhana.Domain.Entities.Curriculum;

namespace EduKidsGhana.Domain.Entities.Learning;

public class WeakAreaRecord : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public Guid TopicId { get; set; }
    public double WeaknessScore { get; set; } = 0;
    public int FailedAttempts { get; set; } = 0;
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual LearnerProfile Learner { get; set; } = null!;
    public virtual Topic Topic { get; set; } = null!;
}
