using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Domain.Entities.Learning;

namespace EduKidsGhana.Domain.Entities.Curriculum;

public class Topic : BaseAuditableEntity
{
    public Guid SubjectId { get; set; }
    public Guid GradeLevelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public DifficultyLevel DefaultDifficulty { get; set; } = DifficultyLevel.Beginner;
    public bool IsActive { get; set; } = true;
    public virtual Subject Subject { get; set; } = null!;
    public virtual GradeLevel GradeLevel { get; set; } = null!;
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public virtual ICollection<TopicMastery> Masteries { get; set; } = new List<TopicMastery>();
}
