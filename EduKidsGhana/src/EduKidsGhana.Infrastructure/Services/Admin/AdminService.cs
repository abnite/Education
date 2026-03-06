using AutoMapper;
using EduKidsGhana.Application.DTOs.Admin;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.AI;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduKidsGhana.Domain.Entities.Identity;

namespace EduKidsGhana.Infrastructure.Services.Admin;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public AdminService(AppDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _db = db;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var totalLearners = await _db.LearnerProfiles.CountAsync(ct);
        var totalParents = await _db.ParentProfiles.CountAsync(ct);
        var totalLessons = await _db.Lessons.CountAsync(ct);
        var totalQuizzes = await _db.QuizAttempts.CountAsync(a => a.CompletedAt.HasValue, ct);
        var today = DateTime.UtcNow.Date;
        var activeLearners = await _db.LearningSessions.Where(s => s.StartedAt.Date == today).Select(s => s.LearnerId).Distinct().CountAsync(ct);

        var subjectUsage = await _db.LearningSessions
            .Include(s => s.Topic).ThenInclude(t => t.Subject)
            .Where(s => s.Topic != null && s.Topic.Subject != null)
            .GroupBy(s => s.Topic!.Subject.Name)
            .Select(g => new SubjectUsageDto { SubjectName = g.Key, SessionCount = g.Count() })
            .ToListAsync(ct);

        return new AdminDashboardDto
        {
            TotalLearners = totalLearners,
            TotalParents = totalParents,
            TotalLessons = totalLessons,
            TotalQuizzesCompleted = totalQuizzes,
            ActiveLearnersToday = activeLearners,
            SubjectUsage = subjectUsage
        };
    }

    public async Task<List<UserSummaryDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _userManager.Users.Where(u => !u.IsDeleted).OrderByDescending(u => u.CreatedAt).ToListAsync(ct);
        var result = new List<UserSummaryDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserSummaryDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }
        return result;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        user.IsActive = false;
        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> UpdateAISettingsAsync(AISettingsDto dto, CancellationToken ct = default)
    {
        var existing = await _db.AIProviderSettings.Where(s => s.IsDefault).FirstOrDefaultAsync(ct);
        if (existing == null)
        {
            _db.AIProviderSettings.Add(new AIProviderSetting
            {
                Name = dto.ProviderType.ToString(),
                ProviderType = dto.ProviderType,
                IsEnabled = dto.IsEnabled,
                IsDefault = true,
                ApiKey = dto.ApiKey,
                ApiEndpoint = dto.ApiEndpoint,
                ModelName = dto.ModelName,
                MaxTokens = dto.MaxTokens,
                Temperature = dto.Temperature,
                TimeoutSeconds = dto.TimeoutSeconds,
                RetryCount = dto.RetryCount
            });
        }
        else
        {
            existing.ProviderType = dto.ProviderType;
            existing.IsEnabled = dto.IsEnabled;
            existing.ApiKey = dto.ApiKey;
            existing.ApiEndpoint = dto.ApiEndpoint;
            existing.ModelName = dto.ModelName;
            existing.MaxTokens = dto.MaxTokens;
            existing.Temperature = dto.Temperature;
            existing.TimeoutSeconds = dto.TimeoutSeconds;
            existing.RetryCount = dto.RetryCount;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<AISettingsDto?> GetAISettingsAsync(CancellationToken ct = default)
    {
        var setting = await _db.AIProviderSettings.Where(s => s.IsDefault).FirstOrDefaultAsync(ct);
        if (setting == null) return null;
        return new AISettingsDto
        {
            Id = setting.Id,
            ProviderType = setting.ProviderType,
            IsEnabled = setting.IsEnabled,
            ApiKey = setting.ApiKey != null ? "***configured***" : null,
            ApiEndpoint = setting.ApiEndpoint,
            ModelName = setting.ModelName,
            MaxTokens = setting.MaxTokens,
            Temperature = setting.Temperature,
            TimeoutSeconds = setting.TimeoutSeconds,
            RetryCount = setting.RetryCount
        };
    }

    public async Task<AnalyticsReportDto> GetAnalyticsReportAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var sessions = await _db.LearningSessions.Where(s => s.StartedAt >= from && s.StartedAt <= to).ToListAsync(ct);
        var attempts = await _db.QuizAttempts.Where(a => a.StartedAt >= from && a.StartedAt <= to).ToListAsync(ct);

        return new AnalyticsReportDto
        {
            FromDate = from,
            ToDate = to,
            TotalSessions = sessions.Count,
            UniqueLearners = sessions.Select(s => s.LearnerId).Distinct().Count(),
            TotalLessonsCompleted = sessions.Count(s => s.Status == Domain.Enums.SessionStatus.Completed),
            TotalQuizzesCompleted = attempts.Count(a => a.CompletedAt.HasValue),
            AverageQuizScore = attempts.Any() ? attempts.Average(a => a.TotalQuestions > 0 ? (double)a.CorrectAnswers / a.TotalQuestions * 100 : 0) : 0,
            TotalMinutesStudied = sessions.Sum(s => s.DurationMinutes)
        };
    }
}
