namespace EduKidsGhana.Domain.Entities.Curriculum;

public class LessonMedia : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public long? FileSizeBytes { get; set; }
    public int SortOrder { get; set; } = 0;
    public virtual Lesson Lesson { get; set; } = null!;
}
