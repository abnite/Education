using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Domain.Entities.Assessment;

public class GeneratedQuiz : BaseAuditableEntity
{
    public Guid? TemplateId { get; set; }
    public Guid LearnerId { get; set; }
    public Guid TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public GenerationSourceType SourceType { get; set; } = GenerationSourceType.RuleBased;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public int TimeLimitSeconds { get; set; } = 300;
    public bool IsCompleted { get; set; } = false;
    public DateTime? ExpiresAt { get; set; }
    public virtual QuizTemplate? Template { get; set; }
    public virtual LearnerProfile Learner { get; set; } = null!;
    public virtual ICollection<GeneratedQuizQuestion> Questions { get; set; } = new List<GeneratedQuizQuestion>();
    public virtual ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
