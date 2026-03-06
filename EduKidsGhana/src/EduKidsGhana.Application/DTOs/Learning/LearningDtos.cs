using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Application.DTOs.Curriculum;

namespace EduKidsGhana.Application.DTOs.Learning;

public class LessonPlayerPayloadDto
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public int EstimatedMinutes { get; set; }
    public List<LessonObjectiveDto> Objectives { get; set; } = new();
    public List<LessonSectionDto> Sections { get; set; } = new();
    public List<LessonExampleDto> Examples { get; set; } = new();
    public List<LessonAudioDto> AudioFiles { get; set; } = new();
    public string? AIEnhancedExplanation { get; set; }
    public bool HasQuiz { get; set; }
    public Guid? QuizTemplateId { get; set; }
}

public class NextRecommendationDto
{
    public Guid? LessonId { get; set; }
    public Guid? TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public RecommendationType Type { get; set; }
    public string? Reason { get; set; }
    public string? SubjectName { get; set; }
    public string? ColourHex { get; set; }
}

public class SessionResultDto
{
    public Guid SessionId { get; set; }
    public bool Started { get; set; }
}

public class RecommendationDto
{
    public Guid Id { get; set; }
    public Guid? TopicId { get; set; }
    public Guid? LessonId { get; set; }
    public RecommendationType RecommendationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public int Priority { get; set; }
}

public class RevisionItemDto
{
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int FailureCount { get; set; }
    public DateTime? NextRevisionAt { get; set; }
}

public class WeakAreaDto
{
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public double WeaknessScore { get; set; }
    public int FailedAttempts { get; set; }
}
