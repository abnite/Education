using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Domain.Entities.Curriculum;

namespace EduKidsGhana.Domain.Entities.Assessment;

public class QuizTemplate : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuizType QuizType { get; set; } = QuizType.Practice;
    public int QuestionCount { get; set; } = 5;
    public int TimeLimitSeconds { get; set; } = 300;
    public int PassMarkPercent { get; set; } = 70;
    public bool IsActive { get; set; } = true;
    public virtual Lesson Lesson { get; set; } = null!;
    public virtual ICollection<GeneratedQuiz> GeneratedQuizzes { get; set; } = new List<GeneratedQuiz>();
}
