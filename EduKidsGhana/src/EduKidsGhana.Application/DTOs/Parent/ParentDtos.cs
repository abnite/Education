using EduKidsGhana.Application.DTOs.Learner;

namespace EduKidsGhana.Application.DTOs.Parent;

public class ParentDashboardDto
{
    public Guid ParentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public List<ChildSummaryDto> Children { get; set; } = new();
}

public class ChildSummaryDto
{
    public Guid LearnerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public int TotalPoints { get; set; }
    public int CurrentStreak { get; set; }
    public DateTime? LastStudyDate { get; set; }
    public double OverallProgress { get; set; }
}

public class ChildProgressDto
{
    public LearnerProfileDto Learner { get; set; } = null!;
    public int TotalLessonsCompleted { get; set; }
    public int TotalQuizzesCompleted { get; set; }
    public int TotalMinutesStudied { get; set; }
    public double AverageScore { get; set; }
    public List<SubjectProgressDto> SubjectBreakdown { get; set; } = new();
    public List<WeakTopicDto> WeakTopics { get; set; } = new();
    public List<StrongTopicDto> StrongTopics { get; set; } = new();
}

public class WeakTopicDto
{
    public string TopicName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public double MasteryScore { get; set; }
}

public class StrongTopicDto
{
    public string TopicName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string MasteryLevel { get; set; } = string.Empty;
}

public record LinkChildDto(Guid LearnerProfileId);
