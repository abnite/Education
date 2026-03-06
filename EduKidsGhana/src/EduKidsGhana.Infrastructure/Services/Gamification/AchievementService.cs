using EduKidsGhana.Application.DTOs.Gamification;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.Gamification;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Gamification;

public class AchievementService : IAchievementService
{
    private readonly AppDbContext _db;
    private readonly IRewardService _rewardService;
    private readonly ILogger<AchievementService> _logger;

    public AchievementService(AppDbContext db, IRewardService rewardService, ILogger<AchievementService> logger)
    {
        _db = db;
        _rewardService = rewardService;
        _logger = logger;
    }

    public async Task<List<AchievementDto>> GetLearnerAchievementsAsync(Guid learnerId, CancellationToken ct = default)
    {
        var all = await _db.Achievements.Where(a => a.IsActive).ToListAsync(ct);
        var earned = await _db.LearnerAchievements
            .Where(la => la.LearnerId == learnerId)
            .ToListAsync(ct);

        return all.Select(a =>
        {
            var e = earned.FirstOrDefault(x => x.AchievementId == a.Id);
            return new AchievementDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                BadgeCode = a.BadgeCode,
                IconUrl = a.IconUrl,
                Category = a.Category,
                IsEarned = e != null,
                EarnedAt = e?.EarnedAt
            };
        }).ToList();
    }

    public async Task CheckAndAwardAchievementsAsync(Guid learnerId, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        if (learner == null) return;

        var earnedIds = await _db.LearnerAchievements
            .Where(la => la.LearnerId == learnerId)
            .Select(la => la.AchievementId)
            .ToListAsync(ct);

        var allAchievements = await _db.Achievements
            .Where(a => a.IsActive && !earnedIds.Contains(a.Id))
            .ToListAsync(ct);

        var quizCount = await _db.QuizAttempts.CountAsync(a => a.LearnerId == learnerId && a.IsPassed, ct);
        var masteredCount = await _db.TopicMasteries.CountAsync(m => m.LearnerId == learnerId && m.MasteryLevel == MasteryLevel.Mastered, ct);

        foreach (var achievement in allAchievements)
        {
            bool shouldAward = achievement.BadgeCode switch
            {
                "FIRST_QUIZ" => quizCount >= 1,
                "QUIZ_10" => quizCount >= 10,
                "STREAK_7" => learner.CurrentStreak >= 7,
                "STREAK_30" => learner.CurrentStreak >= 30,
                "MASTERED_1" => masteredCount >= 1,
                "MASTERED_5" => masteredCount >= 5,
                "POINTS_100" => learner.TotalPoints >= 100,
                "POINTS_500" => learner.TotalPoints >= 500,
                "POINTS_1000" => learner.TotalPoints >= 1000,
                _ => false
            };

            if (shouldAward)
            {
                _db.LearnerAchievements.Add(new LearnerAchievement
                {
                    LearnerId = learnerId,
                    AchievementId = achievement.Id,
                    EarnedAt = DateTime.UtcNow
                });
                await _rewardService.AwardPointsAsync(learnerId, 25, RewardType.AchievementUnlocked, $"Badge earned: {achievement.Name}", achievement.Id.ToString(), ct);
                _logger.LogInformation("Achievement {Badge} awarded to learner {LearnerId}", achievement.BadgeCode, learnerId);
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<AchievementDto>> GetNewlyUnlockedAsync(Guid learnerId, CancellationToken ct = default)
    {
        return await _db.LearnerAchievements
            .Include(la => la.Achievement)
            .Where(la => la.LearnerId == learnerId && !la.IsDisplayed)
            .Select(la => new AchievementDto
            {
                Id = la.Achievement.Id,
                Name = la.Achievement.Name,
                Description = la.Achievement.Description,
                BadgeCode = la.Achievement.BadgeCode,
                IconUrl = la.Achievement.IconUrl,
                Category = la.Achievement.Category,
                IsEarned = true,
                EarnedAt = la.EarnedAt
            })
            .ToListAsync(ct);
    }
}
