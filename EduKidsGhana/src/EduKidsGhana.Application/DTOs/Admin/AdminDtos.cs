using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.DTOs.Admin;

public class AdminDashboardDto
{
    public int TotalLearners { get; set; }
    public int TotalParents { get; set; }
    public int TotalLessons { get; set; }
    public int TotalQuizzesCompleted { get; set; }
    public int ActiveLearnersToday { get; set; }
    public double AverageDailyMinutes { get; set; }
    public List<SubjectUsageDto> SubjectUsage { get; set; } = new();
    public List<RecentRegistrationDto> RecentRegistrations { get; set; } = new();
}

public class SubjectUsageDto
{
    public string SubjectName { get; set; } = string.Empty;
    public int SessionCount { get; set; }
    public double AverageScore { get; set; }
}

public class RecentRegistrationDto
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AISettingsDto
{
    public Guid? Id { get; set; }
    public AIProviderType ProviderType { get; set; }
    public bool IsEnabled { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiEndpoint { get; set; }
    public string? ModelName { get; set; }
    public int MaxTokens { get; set; } = 500;
    public double Temperature { get; set; } = 0.7;
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
}

public class AnalyticsReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalSessions { get; set; }
    public int UniqueLearners { get; set; }
    public int TotalLessonsCompleted { get; set; }
    public int TotalQuizzesCompleted { get; set; }
    public double AverageQuizScore { get; set; }
    public int TotalMinutesStudied { get; set; }
    public List<DailyActivityDto> DailyActivity { get; set; } = new();
}

public class DailyActivityDto
{
    public DateTime Date { get; set; }
    public int Sessions { get; set; }
    public int LessonsCompleted { get; set; }
    public int QuizzesCompleted { get; set; }
}
