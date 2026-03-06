using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Domain.Entities.Users;
using EduKidsGhana.Domain.Entities.Curriculum;

namespace EduKidsGhana.Domain.Entities.Learning;

public class TopicMastery : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public Guid TopicId { get; set; }
    public MasteryLevel MasteryLevel { get; set; } = MasteryLevel.NotStarted;
    public double MasteryScore { get; set; } = 0;
    public int AttemptsCount { get; set; } = 0;
    public int ConsecutiveCorrect { get; set; } = 0;
    public DateTime? LastAttemptAt { get; set; }
    public DateTime? MasteredAt { get; set; }
    public virtual LearnerProfile Learner { get; set; } = null!;
    public virtual Topic Topic { get; set; } = null!;
}
