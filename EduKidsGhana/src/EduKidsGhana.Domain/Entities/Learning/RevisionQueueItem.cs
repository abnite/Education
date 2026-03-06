using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Domain.Entities.Learning;

public class RevisionQueueItem : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public Guid TopicId { get; set; }
    public Guid? QuestionId { get; set; }
    public int FailureCount { get; set; } = 1;
    public bool IsResolved { get; set; } = false;
    public DateTime? NextRevisionAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public virtual LearnerProfile Learner { get; set; } = null!;
}
