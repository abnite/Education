using AutoMapper;
using EduKidsGhana.Application.DTOs.Progress;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Constants;
using EduKidsGhana.Domain.Entities.Learning;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Progress;

public class ProgressTrackingService : IProgressTrackingService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IRewardService _rewardService;
    private readonly ILogger<ProgressTrackingService> _logger;

    public ProgressTrackingService(AppDbContext db, IMapper mapper, IRewardService rewardService, ILogger<ProgressTrackingService> logger)
    {
        _db = db;
        _mapper = mapper;
        _rewardService = rewardService;
        _logger = logger;
    }

    public async Task<LearnerProgressSummaryDto> GetProgressSummaryAsync(Guid learnerId, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct)
            ?? throw new InvalidOperationException("Learner not found.");

        var sessions = await _db.LearningSessions.Where(s => s.LearnerId == learnerId).ToListAsync(ct);
        var attempts = await _db.QuizAttempts.Where(a => a.LearnerId == learnerId).ToListAsync(ct);

        return new LearnerProgressSummaryDto
        {
            LearnerId = learnerId,
            DisplayName = learner.DisplayName,
            GradeLevel = learner.GradeLevel,
            TotalPoints = learner.TotalPoints,
            CurrentStreak = learner.CurrentStreak,
            LongestStreak = learner.LongestStreak,
            TotalLessonsCompleted = sessions.Count(s => s.Status == SessionStatus.Completed),
            TotalQuizzesCompleted = attempts.Count(a => a.CompletedAt.HasValue),
            TotalMinutesStudied = sessions.Sum(s => s.DurationMinutes),
            OverallAverageScore = attempts.Any() ? attempts.Average(a => a.TotalQuestions > 0 ? (double)a.CorrectAnswers / a.TotalQuestions * 100 : 0) : 0,
            LastStudyDate = learner.LastStudyDate
        };
    }

    public async Task<List<TopicMasteryDto>> GetTopicMasteriesAsync(Guid learnerId, CancellationToken ct = default)
    {
        return await _db.TopicMasteries
            .Include(m => m.Topic).ThenInclude(t => t.Subject)
            .Where(m => m.LearnerId == learnerId)
            .Select(m => new TopicMasteryDto
            {
                TopicId = m.TopicId,
                TopicName = m.Topic.Name,
                SubjectName = m.Topic.Subject.Name,
                SubjectColour = m.Topic.Subject.ColourHex,
                MasteryLevel = m.MasteryLevel,
                MasteryScore = m.MasteryScore,
                AttemptsCount = m.AttemptsCount,
                LastAttemptAt = m.LastAttemptAt,
                MasteredAt = m.MasteredAt
            })
            .OrderByDescending(m => m.MasteryScore)
            .ToListAsync(ct);
    }

    public async Task RecordLessonCompletionAsync(Guid learnerId, Guid lessonId, int minutesSpent, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons.Include(l => l.Topic).FirstOrDefaultAsync(l => l.Id == lessonId, ct);
        if (lesson == null) return;

        var record = await _db.ProgressRecords
            .FirstOrDefaultAsync(r => r.LearnerId == learnerId && r.TopicId == lesson.TopicId, ct);

        if (record == null)
        {
            record = new ProgressRecord { LearnerId = learnerId, TopicId = lesson.TopicId };
            _db.ProgressRecords.Add(record);
        }

        record.LessonsCompleted++;
        record.TotalMinutesSpent += minutesSpent;
        record.LastActivityAt = DateTime.UtcNow;
        record.UpdatedAt = DateTime.UtcNow;

        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        if (learner != null)
        {
            learner.LastStudyDate = DateTime.UtcNow;
            learner.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        await _rewardService.AwardPointsAsync(learnerId, DomainConstants.Gamification.PointsPerLesson, RewardType.LessonCompletion, $"Completed lesson", lessonId.ToString(), ct);
    }

    public async Task RecordQuizResultAsync(Guid learnerId, Guid topicId, double scorePercent, int pointsEarned, CancellationToken ct = default)
    {
        var record = await _db.ProgressRecords
            .FirstOrDefaultAsync(r => r.LearnerId == learnerId && r.TopicId == topicId, ct);

        if (record == null)
        {
            record = new ProgressRecord { LearnerId = learnerId, TopicId = topicId };
            _db.ProgressRecords.Add(record);
        }

        record.QuizzesCompleted++;
        record.TotalPointsEarned += pointsEarned;
        record.AverageScore = record.QuizzesCompleted == 1
            ? scorePercent
            : (record.AverageScore + scorePercent) / 2;
        record.LastActivityAt = DateTime.UtcNow;
        record.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateStreakAsync(Guid learnerId, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        if (learner == null) return;

        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        if (learner.LastStudyDate?.Date == today) return; // Already counted today

        if (learner.LastStudyDate?.Date == yesterday)
            learner.CurrentStreak++;
        else
            learner.CurrentStreak = 1;

        if (learner.CurrentStreak > learner.LongestStreak)
            learner.LongestStreak = learner.CurrentStreak;

        learner.LastStudyDate = DateTime.UtcNow;
        learner.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        if (learner.CurrentStreak > 0)
            await _rewardService.AwardPointsAsync(learnerId, DomainConstants.Gamification.StreakBonusPoints, RewardType.StreakBonus, $"Streak day {learner.CurrentStreak}", ct: ct);
    }
}
