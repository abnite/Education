using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Domain.Entities.Gamification;

public class LearnerAchievement : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    public bool IsDisplayed { get; set; } = false;
    public virtual LearnerProfile Learner { get; set; } = null!;
    public virtual Achievement Achievement { get; set; } = null!;
}
