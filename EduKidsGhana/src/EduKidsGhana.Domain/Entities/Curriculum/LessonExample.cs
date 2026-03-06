namespace EduKidsGhana.Domain.Entities.Curriculum;

public class LessonExample : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Solution { get; set; }
    public string? Explanation { get; set; }
    public int SortOrder { get; set; } = 0;
    public virtual Lesson Lesson { get; set; } = null!;
}
