using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Domain.Entities.Curriculum;

public class LessonAudio : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public AudioType AudioType { get; set; } = AudioType.LessonNarration;
    public int? DurationSeconds { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual Lesson Lesson { get; set; } = null!;
}
