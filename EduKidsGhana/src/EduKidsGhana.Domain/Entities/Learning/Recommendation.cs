using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Domain.Entities.Learning;

public class Recommendation : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public Guid? TopicId { get; set; }
    public Guid? LessonId { get; set; }
    public RecommendationType RecommendationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public int Priority { get; set; } = 0;
    public bool IsActioned { get; set; } = false;
    public DateTime? ActionedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public virtual LearnerProfile Learner { get; set; } = null!;
}
