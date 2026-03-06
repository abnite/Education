namespace EduKidsGhana.Domain.Entities.Users;

public class ParentLearnerLink : BaseAuditableEntity
{
    public Guid ParentProfileId { get; set; }
    public Guid LearnerProfileId { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ParentProfile Parent { get; set; } = null!;
    public virtual LearnerProfile Learner { get; set; } = null!;
}
