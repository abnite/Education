namespace EduKidsGhana.Application.DTOs.Learner;

public class LearnerProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Age { get; set; }
    public int GradeLevel { get; set; }
    public string? AvatarCode { get; set; }
    public int TotalPoints { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public bool AudioEnabled { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public DateTime? LastStudyDate { get; set; }
}

public record CreateLearnerDto(
    string DisplayName,
    int Age,
    int GradeLevel,
    string? AvatarCode,
    bool AudioEnabled = true);

public record UpdateLearnerDto(
    string DisplayName,
    int Age,
    int GradeLevel,
    string? AvatarCode,
    bool AudioEnabled,
    string PreferredLanguage);

public class LearnerDashboardDto
{
    public LearnerProfileDto Profile { get; set; } = null!;
    public int TodayMinutes { get; set; }
    public int DailyGoalMinutes { get; set; }
    public int CurrentStreak { get; set; }
    public List<SubjectProgressDto> SubjectProgress { get; set; } = new();
    public List<RecentActivityDto> RecentActivity { get; set; } = new();
    public NextChallengeDto? DailyChallenge { get; set; }
    public List<NewBadgeDto> NewBadges { get; set; } = new();
}

public class SubjectProgressDto
{
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string ColourHex { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int CompletedTopics { get; set; }
    public int TotalTopics { get; set; }
    public double ProgressPercent { get; set; }
}

public class RecentActivityDto
{
    public string ActivityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int PointsEarned { get; set; }
    public DateTime ActivityAt { get; set; }
}

public class NextChallengeDto
{
    public Guid ChallengeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RewardPoints { get; set; }
}

public class NewBadgeDto
{
    public string BadgeCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
}

public class AvatarDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
