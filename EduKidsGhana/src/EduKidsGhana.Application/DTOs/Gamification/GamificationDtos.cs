using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.DTOs.Gamification;

public class AchievementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BadgeCode { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public BadgeCategory Category { get; set; }
    public bool IsEarned { get; set; }
    public DateTime? EarnedAt { get; set; }
}

public class ChallengeProgressDto
{
    public Guid ChallengeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TargetValue { get; set; }
    public int CurrentValue { get; set; }
    public int RewardPoints { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime ChallengeDate { get; set; }
}

public class StreakInfoDto
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public bool IsAtRisk { get; set; }
    public DateTime? LastStudyDate { get; set; }
}

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarCode { get; set; }
    public int TotalPoints { get; set; }
    public int GradeLevel { get; set; }
}
