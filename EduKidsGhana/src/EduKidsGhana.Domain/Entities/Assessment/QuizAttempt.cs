using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Domain.Entities.Assessment;

public class QuizAttempt : BaseAuditableEntity
{
    public Guid QuizId { get; set; }
    public Guid LearnerId { get; set; }
    public int Score { get; set; } = 0;
    public int TotalPoints { get; set; } = 0;
    public int CorrectAnswers { get; set; } = 0;
    public int TotalQuestions { get; set; } = 0;
    public int DurationSeconds { get; set; } = 0;
    public bool IsPassed { get; set; } = false;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public virtual GeneratedQuiz Quiz { get; set; } = null!;
    public virtual LearnerProfile Learner { get; set; } = null!;
    public virtual ICollection<LearnerAnswer> Answers { get; set; } = new List<LearnerAnswer>();
}
