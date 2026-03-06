using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.DTOs.Progress;

public class LearnerProgressSummaryDto
{
    public Guid LearnerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public int TotalPoints { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalLessonsCompleted { get; set; }
    public int TotalQuizzesCompleted { get; set; }
    public int TotalMinutesStudied { get; set; }
    public double OverallAverageScore { get; set; }
    public DateTime? LastStudyDate { get; set; }
}

public class TopicMasteryDto
{
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectColour { get; set; } = string.Empty;
    public MasteryLevel MasteryLevel { get; set; }
    public double MasteryScore { get; set; }
    public int AttemptsCount { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public DateTime? MasteredAt { get; set; }
}

public class SubjectMasterySummaryDto
{
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string ColourHex { get; set; } = string.Empty;
    public int TotalTopics { get; set; }
    public int MasteredTopics { get; set; }
    public int ProficientTopics { get; set; }
    public double AverageMasteryScore { get; set; }
}
