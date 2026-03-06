using EduKidsGhana.Application.DTOs.Learning;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.Learning;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Learning;

public class LessonRecommendationService : ILessonRecommendationService
{
    private readonly AppDbContext _db;
    private readonly ILogger<LessonRecommendationService> _logger;

    public LessonRecommendationService(AppDbContext db, ILogger<LessonRecommendationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<RecommendationDto>> GetRecommendationsAsync(Guid learnerId, int count = 3, CancellationToken ct = default)
    {
        await GenerateRecommendationsAsync(learnerId, ct);
        return await _db.Recommendations
            .Where(r => r.LearnerId == learnerId && !r.IsActioned && (r.ExpiresAt == null || r.ExpiresAt > DateTime.UtcNow))
            .OrderBy(r => r.Priority)
            .Take(count)
            .Select(r => new RecommendationDto
            {
                Id = r.Id,
                TopicId = r.TopicId,
                LessonId = r.LessonId,
                RecommendationType = r.RecommendationType,
                Title = r.Title,
                Reason = r.Reason,
                Priority = r.Priority
            })
            .ToListAsync(ct);
    }

    public async Task<RecommendationDto?> GetPriorityRecommendationAsync(Guid learnerId, CancellationToken ct = default)
    {
        var recs = await GetRecommendationsAsync(learnerId, 1, ct);
        return recs.FirstOrDefault();
    }

    public async Task GenerateRecommendationsAsync(Guid learnerId, CancellationToken ct = default)
    {
        // Clear old recommendations
        var old = await _db.Recommendations
            .Where(r => r.LearnerId == learnerId && !r.IsActioned && r.CreatedAt < DateTime.UtcNow.AddHours(-1))
            .ToListAsync(ct);
        _db.Recommendations.RemoveRange(old);

        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        if (learner == null) return;

        // Find weak areas for revision
        var weakAreas = await _db.TopicMasteries
            .Include(m => m.Topic)
            .Where(m => m.LearnerId == learnerId && m.MasteryScore < 60 && m.AttemptsCount > 0)
            .OrderBy(m => m.MasteryScore)
            .Take(2)
            .ToListAsync(ct);

        int priority = 0;
        foreach (var weak in weakAreas)
        {
            var lesson = await _db.Lessons
                .Where(l => l.TopicId == weak.TopicId && l.LessonType == Domain.Enums.LessonType.Revision && l.IsActive && l.IsPublished)
                .FirstOrDefaultAsync(ct)
                ?? await _db.Lessons
                .Where(l => l.TopicId == weak.TopicId && l.IsActive && l.IsPublished)
                .FirstOrDefaultAsync(ct);

            if (lesson != null)
            {
                _db.Recommendations.Add(new Recommendation
                {
                    LearnerId = learnerId,
                    TopicId = weak.TopicId,
                    LessonId = lesson.Id,
                    RecommendationType = RecommendationType.Revision,
                    Title = $"Revise: {weak.Topic.Name}",
                    Reason = $"Your score was {weak.MasteryScore:F0}%. Let's practise more!",
                    Priority = priority++,
                    ExpiresAt = DateTime.UtcNow.AddDays(1)
                });
            }
        }

        // Find next new topic
        var masteredTopicIds = await _db.TopicMasteries
            .Where(m => m.LearnerId == learnerId)
            .Select(m => m.TopicId)
            .ToListAsync(ct);

        var nextLesson = await _db.Lessons
            .Include(l => l.Topic)
            .Where(l => l.Topic.GradeLevel.Grade == learner.GradeLevel
                && !masteredTopicIds.Contains(l.TopicId)
                && l.IsActive && l.IsPublished)
            .OrderBy(l => l.Topic.SortOrder).ThenBy(l => l.SortOrder)
            .FirstOrDefaultAsync(ct);

        if (nextLesson != null)
        {
            _db.Recommendations.Add(new Recommendation
            {
                LearnerId = learnerId,
                TopicId = nextLesson.TopicId,
                LessonId = nextLesson.Id,
                RecommendationType = RecommendationType.NewLesson,
                Title = nextLesson.Title,
                Reason = "Continue your learning journey!",
                Priority = priority,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            });
        }

        await _db.SaveChangesAsync(ct);
    }
}
