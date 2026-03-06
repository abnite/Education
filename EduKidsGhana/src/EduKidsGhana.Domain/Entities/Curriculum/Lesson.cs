using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Domain.Entities.Assessment;

namespace EduKidsGhana.Domain.Entities.Curriculum;

public class Lesson : BaseAuditableEntity
{
    public Guid TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public LessonType LessonType { get; set; } = LessonType.Core;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public int EstimatedMinutes { get; set; } = 15;
    public int SortOrder { get; set; } = 0;
    public bool IsPublished { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public virtual Topic Topic { get; set; } = null!;
    public virtual ICollection<LessonSection> Sections { get; set; } = new List<LessonSection>();
    public virtual ICollection<LessonObjective> Objectives { get; set; } = new List<LessonObjective>();
    public virtual ICollection<LessonExample> Examples { get; set; } = new List<LessonExample>();
    public virtual ICollection<LessonAudio> AudioFiles { get; set; } = new List<LessonAudio>();
    public virtual ICollection<LessonMedia> MediaFiles { get; set; } = new List<LessonMedia>();
    public virtual ICollection<QuizTemplate> QuizTemplates { get; set; } = new List<QuizTemplate>();
}
