using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Domain.Entities.Assessment;

public class GeneratedQuizQuestion : BaseAuditableEntity
{
    public Guid QuizId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public string? ImageUrl { get; set; }
    public QuestionType QuestionType { get; set; } = QuestionType.MultipleChoice;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string? HintText { get; set; }
    public int Points { get; set; } = 10;
    public int SortOrder { get; set; } = 0;
    public virtual GeneratedQuiz Quiz { get; set; } = null!;
    public virtual ICollection<GeneratedQuizOption> Options { get; set; } = new List<GeneratedQuizOption>();
    public virtual ICollection<LearnerAnswer> LearnerAnswers { get; set; } = new List<LearnerAnswer>();
}
