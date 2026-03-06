using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Domain.Entities.Assessment;

public class HintRecord : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public Guid QuestionId { get; set; }
    public string HintText { get; set; } = string.Empty;
    public string SourceType { get; set; } = "Rule";
    public virtual LearnerProfile Learner { get; set; } = null!;
}
