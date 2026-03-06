using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Domain.Entities.Learning;

public class LearningSession : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public Guid? TopicId { get; set; }
    public Guid? LessonId { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Active;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public int DurationMinutes { get; set; } = 0;
    public int PointsEarned { get; set; } = 0;
    public virtual LearnerProfile Learner { get; set; } = null!;
}
