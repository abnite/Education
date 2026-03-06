namespace EduKidsGhana.Domain.Entities.Curriculum;

public class LessonSection : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ContentHtml { get; set; }
    public int SortOrder { get; set; } = 0;
    public string? AudioUrl { get; set; }
    public bool HasInteraction { get; set; } = false;
    public virtual Lesson Lesson { get; set; } = null!;
}
