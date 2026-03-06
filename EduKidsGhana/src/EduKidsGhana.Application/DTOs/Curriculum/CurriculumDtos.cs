using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.DTOs.Curriculum;

public class SubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string ColourHex { get; set; } = string.Empty;
    public SubjectType SubjectType { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public record CreateSubjectDto(
    string Name,
    string Code,
    string? Description,
    string? IconUrl,
    string ColourHex,
    SubjectType SubjectType,
    int SortOrder = 0);

public class GradeLevelDto
{
    public Guid Id { get; set; }
    public int Grade { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
}

public class TopicDto
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public Guid GradeLevelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public DifficultyLevel DefaultDifficulty { get; set; }
    public bool IsActive { get; set; }
}

public record CreateTopicDto(
    Guid SubjectId,
    Guid GradeLevelId,
    string Name,
    string? Description,
    DifficultyLevel DefaultDifficulty,
    int SortOrder = 0);

public class LessonSummaryDto
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public LessonType LessonType { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int EstimatedMinutes { get; set; }
    public bool IsPublished { get; set; }
}

public class LessonDetailDto
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public LessonType LessonType { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int EstimatedMinutes { get; set; }
    public bool IsPublished { get; set; }
    public List<LessonObjectiveDto> Objectives { get; set; } = new();
    public List<LessonSectionDto> Sections { get; set; } = new();
    public List<LessonExampleDto> Examples { get; set; } = new();
    public List<LessonAudioDto> AudioFiles { get; set; } = new();
}

public class LessonObjectiveDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class LessonSectionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ContentHtml { get; set; }
    public int SortOrder { get; set; }
    public string? AudioUrl { get; set; }
    public bool HasInteraction { get; set; }
}

public class LessonExampleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Solution { get; set; }
    public string? Explanation { get; set; }
    public int SortOrder { get; set; }
}

public class LessonAudioDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public AudioType AudioType { get; set; }
    public int? DurationSeconds { get; set; }
}

public class CreateLessonDto
{
    public Guid TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public LessonType LessonType { get; set; } = LessonType.Core;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public int EstimatedMinutes { get; set; } = 15;
    public int SortOrder { get; set; } = 0;
    public List<string> Objectives { get; set; } = new();
    public List<CreateLessonSectionDto> Sections { get; set; } = new();
    public List<CreateLessonExampleDto> Examples { get; set; } = new();
}

public record CreateLessonSectionDto(string Title, string Content, string? ContentHtml, int SortOrder, string? AudioUrl);
public record CreateLessonExampleDto(string Title, string Content, string? Solution, string? Explanation, int SortOrder);
