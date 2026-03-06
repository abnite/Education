using AutoMapper;
using EduKidsGhana.Application.DTOs.Learner;
using EduKidsGhana.Application.DTOs.Parent;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Curriculum;

public class ParentService : IParentService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IProgressTrackingService _progressService;

    public ParentService(AppDbContext db, IMapper mapper, IProgressTrackingService progressService)
    {
        _db = db;
        _mapper = mapper;
        _progressService = progressService;
    }

    public async Task<ParentDashboardDto> GetDashboardAsync(Guid parentUserId, CancellationToken ct = default)
    {
        var parent = await _db.ParentProfiles
            .Include(p => p.User)
            .Include(p => p.LearnerLinks).ThenInclude(l => l.Learner)
            .FirstOrDefaultAsync(p => p.UserId == parentUserId, ct)
            ?? throw new InvalidOperationException("Parent profile not found.");

        var children = parent.LearnerLinks.Where(l => l.IsActive).Select(l => new ChildSummaryDto
        {
            LearnerId = l.Learner.Id,
            DisplayName = l.Learner.DisplayName,
            GradeLevel = l.Learner.GradeLevel,
            TotalPoints = l.Learner.TotalPoints,
            CurrentStreak = l.Learner.CurrentStreak,
            LastStudyDate = l.Learner.LastStudyDate
        }).ToList();

        return new ParentDashboardDto
        {
            ParentId = parent.Id,
            FirstName = parent.User.FirstName,
            Children = children
        };
    }

    public async Task<bool> LinkChildAsync(Guid parentUserId, Guid learnerProfileId, CancellationToken ct = default)
    {
        var parent = await _db.ParentProfiles.FirstOrDefaultAsync(p => p.UserId == parentUserId, ct);
        if (parent == null) return false;
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerProfileId }, ct);
        if (learner == null) return false;
        var existing = await _db.ParentLearnerLinks.AnyAsync(l => l.ParentProfileId == parent.Id && l.LearnerProfileId == learnerProfileId, ct);
        if (existing) return true;
        _db.ParentLearnerLinks.Add(new Domain.Entities.Users.ParentLearnerLink { ParentProfileId = parent.Id, LearnerProfileId = learnerProfileId });
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<ChildProgressDto>> GetChildrenProgressAsync(Guid parentUserId, CancellationToken ct = default)
    {
        var parent = await _db.ParentProfiles.Include(p => p.LearnerLinks).ThenInclude(l => l.Learner).FirstOrDefaultAsync(p => p.UserId == parentUserId, ct);
        if (parent == null) return new List<ChildProgressDto>();
        var result = new List<ChildProgressDto>();
        foreach (var link in parent.LearnerLinks.Where(l => l.IsActive))
        {
            var summary = await _progressService.GetProgressSummaryAsync(link.LearnerId, ct);
            result.Add(new ChildProgressDto
            {
                Learner = _mapper.Map<LearnerProfileDto>(link.Learner),
                TotalLessonsCompleted = summary.TotalLessonsCompleted,
                TotalQuizzesCompleted = summary.TotalQuizzesCompleted,
                TotalMinutesStudied = summary.TotalMinutesStudied,
                AverageScore = summary.OverallAverageScore
            });
        }
        return result;
    }

    public async Task<ChildProgressDto> GetChildProgressDetailAsync(Guid parentUserId, Guid learnerProfileId, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerProfileId }, ct)
            ?? throw new InvalidOperationException("Learner not found.");
        var summary = await _progressService.GetProgressSummaryAsync(learnerProfileId, ct);
        var masteries = await _progressService.GetTopicMasteriesAsync(learnerProfileId, ct);

        return new ChildProgressDto
        {
            Learner = _mapper.Map<LearnerProfileDto>(learner),
            TotalLessonsCompleted = summary.TotalLessonsCompleted,
            TotalQuizzesCompleted = summary.TotalQuizzesCompleted,
            TotalMinutesStudied = summary.TotalMinutesStudied,
            AverageScore = summary.OverallAverageScore,
            WeakTopics = masteries.Where(m => m.MasteryScore < 50 && m.AttemptsCount > 0).Select(m => new WeakTopicDto { TopicName = m.TopicName, SubjectName = m.SubjectName, MasteryScore = m.MasteryScore }).Take(5).ToList(),
            StrongTopics = masteries.Where(m => m.MasteryLevel >= MasteryLevel.Proficient).Select(m => new StrongTopicDto { TopicName = m.TopicName, SubjectName = m.SubjectName, MasteryLevel = m.MasteryLevel.ToString() }).Take(5).ToList()
        };
    }
}
