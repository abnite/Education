namespace EduKidsGhana.Domain.Entities.Assessment;

public class LearnerAnswer : BaseAuditableEntity
{
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string AnswerGiven { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
    public int PointsEarned { get; set; } = 0;
    public int TimeTakenSeconds { get; set; } = 0;
    public bool HintUsed { get; set; } = false;
    public virtual QuizAttempt Attempt { get; set; } = null!;
    public virtual GeneratedQuizQuestion Question { get; set; } = null!;
}
