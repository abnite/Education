namespace EduKidsGhana.Domain.Entities.Curriculum;

public class LessonObjective : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
    public virtual Lesson Lesson { get; set; } = null!;
}
