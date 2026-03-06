using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Domain.Entities.Gamification;

public class DailyChallenge : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? TopicId { get; set; }
    public ChallengeType ChallengeType { get; set; }
    public int TargetValue { get; set; } = 1;
    public int RewardPoints { get; set; } = 50;
    public DateTime ChallengeDate { get; set; }
    public int GradeLevel { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
