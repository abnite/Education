using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.DTOs.Quiz;

public class GenerateQuizRequestDto
{
    public Guid LearnerId { get; set; }
    public Guid TopicId { get; set; }
    public Guid? LessonId { get; set; }
    public QuizType QuizType { get; set; } = QuizType.Practice;
    public DifficultyLevel? Difficulty { get; set; }
    public int QuestionCount { get; set; } = 5;
    public bool UseAI { get; set; } = true;
}

public class GeneratedQuizDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public int TimeLimitSeconds { get; set; }
    public GenerationSourceType SourceType { get; set; }
    public List<QuizQuestionDto> Questions { get; set; } = new();
}

public class QuizQuestionDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public string? ImageUrl { get; set; }
    public QuestionType QuestionType { get; set; }
    public int Points { get; set; }
    public int SortOrder { get; set; }
    public List<QuizOptionDto> Options { get; set; } = new();
}

public class QuizOptionDto
{
    public Guid Id { get; set; }
    public string OptionKey { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
}

public class QuizSubmissionDto
{
    public Guid QuizId { get; set; }
    public Guid LearnerId { get; set; }
    public int DurationSeconds { get; set; }
    public List<AnswerSubmissionDto> Answers { get; set; } = new();
}

public class AnswerSubmissionDto
{
    public Guid QuestionId { get; set; }
    public string AnswerGiven { get; set; } = string.Empty;
    public int TimeTakenSeconds { get; set; }
    public bool HintUsed { get; set; }
}

public class QuizSubmitResultDto
{
    public Guid AttemptId { get; set; }
    public int Score { get; set; }
    public int TotalPoints { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
    public bool IsPassed { get; set; }
    public int PointsEarned { get; set; }
    public string EncouragementMessage { get; set; } = string.Empty;
    public List<string> NewBadges { get; set; } = new();
}

public class QuizReviewDto
{
    public Guid AttemptId { get; set; }
    public int Score { get; set; }
    public double Percentage { get; set; }
    public bool IsPassed { get; set; }
    public List<QuizAnswerReviewDto> AnswerReviews { get; set; } = new();
}

public class QuizAnswerReviewDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string AnswerGiven { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string? Explanation { get; set; }
    public int PointsEarned { get; set; }
}

public class HintRequestDto
{
    public Guid QuestionId { get; set; }
    public Guid LearnerId { get; set; }
}

public class HintResponseDto
{
    public string HintText { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
}
