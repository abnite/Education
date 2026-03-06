using EduKidsGhana.Application.DTOs.Learning;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Constants;
using EduKidsGhana.Domain.Entities.Learning;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Learning;

public class AdaptiveLearningService : IAdaptiveLearningService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AdaptiveLearningService> _logger;

    public AdaptiveLearningService(AppDbContext db, ILogger<AdaptiveLearningService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<DifficultyLevel> DetermineNextDifficultyAsync(Guid learnerId, Guid topicId, CancellationToken ct = default)
    {
        var mastery = await _db.TopicMasteries
            .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.TopicId == topicId, ct);

        if (mastery == null) return DifficultyLevel.Beginner;

        return mastery.MasteryLevel switch
        {
            MasteryLevel.NotStarted => DifficultyLevel.Beginner,
            MasteryLevel.Emerging => DifficultyLevel.Beginner,
            MasteryLevel.Developing => DifficultyLevel.Elementary,
            MasteryLevel.Proficient => DifficultyLevel.Intermediate,
            MasteryLevel.Mastered => DifficultyLevel.Advanced,
            _ => DifficultyLevel.Beginner
        };
    }

    public async Task UpdateMasteryAsync(Guid learnerId, Guid topicId, double scorePercent, CancellationToken ct = default)
    {
        var mastery = await _db.TopicMasteries
            .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.TopicId == topicId, ct);

        if (mastery == null)
        {
            mastery = new TopicMastery
            {
                LearnerId = learnerId,
                TopicId = topicId,
                MasteryScore = scorePercent,
                AttemptsCount = 1,
                LastAttemptAt = DateTime.UtcNow
            };
            _db.TopicMasteries.Add(mastery);
        }
        else
        {
            // Weighted rolling average - recent scores count more
            mastery.MasteryScore = mastery.AttemptsCount == 0
                ? scorePercent
                : (mastery.MasteryScore * 0.6) + (scorePercent * 0.4);
            mastery.AttemptsCount++;
            mastery.LastAttemptAt = DateTime.UtcNow;

            if (scorePercent >= 80)
                mastery.ConsecutiveCorrect++;
            else
                mastery.ConsecutiveCorrect = 0;
        }

        // Update mastery level based on score
        mastery.MasteryLevel = mastery.MasteryScore switch
        {
            >= DomainConstants.Mastery.MasteredThreshold when mastery.AttemptsCount >= DomainConstants.Mastery.MinAttemptsForMastery => MasteryLevel.Mastered,
            >= DomainConstants.Mastery.ProficientThreshold => MasteryLevel.Proficient,
            >= DomainConstants.Mastery.DevelopingThreshold => MasteryLevel.Developing,
            >= DomainConstants.Mastery.EmergingThreshold => MasteryLevel.Emerging,
            _ => MasteryLevel.NotStarted
        };

        if (mastery.MasteryLevel == MasteryLevel.Mastered && mastery.MasteredAt == null)
            mastery.MasteredAt = DateTime.UtcNow;

        mastery.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Mastery updated for learner {LearnerId} topic {TopicId}: {Score}% -> {Level}", learnerId, topicId, scorePercent, mastery.MasteryLevel);
    }

    public async Task<MasteryLevel> GetCurrentMasteryLevelAsync(Guid learnerId, Guid topicId, CancellationToken ct = default)
    {
        var mastery = await _db.TopicMasteries
            .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.TopicId == topicId, ct);
        return mastery?.MasteryLevel ?? MasteryLevel.NotStarted;
    }

    public async Task QueueForRevisionAsync(Guid learnerId, Guid topicId, Guid? questionId = null, CancellationToken ct = default)
    {
        var existing = await _db.RevisionQueueItems
            .FirstOrDefaultAsync(r => r.LearnerId == learnerId && r.TopicId == topicId && !r.IsResolved, ct);

        if (existing != null)
        {
            existing.FailureCount++;
            existing.NextRevisionAt = DateTime.UtcNow.AddHours(existing.FailureCount * 2); // Spaced repetition
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.RevisionQueueItems.Add(new RevisionQueueItem
            {
                LearnerId = learnerId,
                TopicId = topicId,
                QuestionId = questionId,
                FailureCount = 1,
                NextRevisionAt = DateTime.UtcNow.AddHours(2)
            });
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<RevisionItemDto>> GetRevisionQueueAsync(Guid learnerId, CancellationToken ct = default)
    {
        return await _db.RevisionQueueItems
            .Include(r => r.Topic).ThenInclude(t => t.Subject)
            .Where(r => r.LearnerId == learnerId && !r.IsResolved)
            .OrderByDescending(r => r.FailureCount)
            .Select(r => new RevisionItemDto
            {
                TopicId = r.TopicId,
                TopicName = r.Topic.Name,
                SubjectName = r.Topic.Subject.Name,
                FailureCount = r.FailureCount,
                NextRevisionAt = r.NextRevisionAt
            })
            .ToListAsync(ct);
    }

    public async Task<bool> IsReadyForProgressionAsync(Guid learnerId, Guid topicId, CancellationToken ct = default)
    {
        var level = await GetCurrentMasteryLevelAsync(learnerId, topicId, ct);
        return level >= MasteryLevel.Proficient;
    }
}
