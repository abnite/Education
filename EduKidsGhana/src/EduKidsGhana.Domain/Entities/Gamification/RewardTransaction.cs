using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Domain.Entities.Gamification;

public class RewardTransaction : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public int Points { get; set; } = 0;
    public RewardType RewardType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public virtual LearnerProfile Learner { get; set; } = null!;
}
