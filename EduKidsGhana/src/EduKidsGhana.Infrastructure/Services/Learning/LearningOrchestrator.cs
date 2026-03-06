using AutoMapper;
using EduKidsGhana.Application.DTOs.Learning;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.Learning;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using EduKidsGhana.Infrastructure.Services.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Learning;

public class LearningOrchestrator : ILearningOrchestrator
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly AIProviderFactory _aiFactory;
    private readonly ILessonRecommendationService _recommendationService;
    private readonly ILogger<LearningOrchestrator> _logger;

    public LearningOrchestrator(
        AppDbContext db,
        IMapper mapper,
        AIProviderFactory aiFactory,
        ILessonRecommendationService recommendationService,
        ILogger<LearningOrchestrator> logger)
    {
        _db = db;
        _mapper = mapper;
        _aiFactory = aiFactory;
        _recommendationService = recommendationService;
        _logger = logger;
    }

    public async Task<LessonPlayerPayloadDto> GetLessonPlayerPayloadAsync(Guid learnerId, Guid lessonId, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons
            .Include(l => l.Topic).ThenInclude(t => t.Subject)
            .Include(l => l.Sections)
            .Include(l => l.Objectives)
            .Include(l => l.Examples)
            .Include(l => l.AudioFiles)
            .Include(l => l.QuizTemplates)
            .FirstOrDefaultAsync(l => l.Id == lessonId, ct)
            ?? throw new InvalidOperationException("Lesson not found.");

        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct)
            ?? throw new InvalidOperationException("Learner not found.");

        string? aiExplanation = null;
        var aiProvider = await _aiFactory.GetProviderAsync(ct);
        if (aiProvider.IsAvailable && lesson.Sections.Any())
        {
            var content = string.Join("\n", lesson.Sections.OrderBy(s => s.SortOrder).Select(s => s.Content));
            aiExplanation = await aiProvider.GenerateExplanationAsync(lesson.Topic.Name, content, learner.GradeLevel, ct);
        }

        return new LessonPlayerPayloadDto
        {
            LessonId = lesson.Id,
            Title = lesson.Title,
            SubjectName = lesson.Topic.Subject.Name,
            TopicName = lesson.Topic.Name,
            Difficulty = lesson.Difficulty,
            EstimatedMinutes = lesson.EstimatedMinutes,
            Objectives = lesson.Objectives.OrderBy(o => o.SortOrder).Select(o => new Application.DTOs.Curriculum.LessonObjectiveDto { Id = o.Id, Description = o.Description, SortOrder = o.SortOrder }).ToList(),
            Sections = lesson.Sections.OrderBy(s => s.SortOrder).Select(s => new Application.DTOs.Curriculum.LessonSectionDto { Id = s.Id, Title = s.Title, Content = s.Content, ContentHtml = s.ContentHtml, SortOrder = s.SortOrder, AudioUrl = s.AudioUrl, HasInteraction = s.HasInteraction }).ToList(),
            Examples = lesson.Examples.OrderBy(e => e.SortOrder).Select(e => new Application.DTOs.Curriculum.LessonExampleDto { Id = e.Id, Title = e.Title, Content = e.Content, Solution = e.Solution, Explanation = e.Explanation, SortOrder = e.SortOrder }).ToList(),
            AudioFiles = lesson.AudioFiles.Where(a => a.IsActive).Select(a => new Application.DTOs.Curriculum.LessonAudioDto { Id = a.Id, Title = a.Title, FileUrl = a.FileUrl, AudioType = a.AudioType, DurationSeconds = a.DurationSeconds }).ToList(),
            AIEnhancedExplanation = aiExplanation,
            HasQuiz = lesson.QuizTemplates.Any(qt => qt.IsActive),
            QuizTemplateId = lesson.QuizTemplates.FirstOrDefault(qt => qt.IsActive)?.Id
        };
    }

    public async Task<NextRecommendationDto> GetNextRecommendationAsync(Guid learnerId, CancellationToken ct = default)
    {
        var rec = await _recommendationService.GetPriorityRecommendationAsync(learnerId, ct);
        if (rec != null)
        {
            return new NextRecommendationDto
            {
                LessonId = rec.LessonId,
                TopicId = rec.TopicId,
                Title = rec.Title,
                Type = rec.RecommendationType,
                Reason = rec.Reason
            };
        }

        // Auto-pick the next unstarted lesson
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        if (learner == null) return new NextRecommendationDto { Title = "Start Learning!" };

        var startedTopics = await _db.TopicMasteries.Where(m => m.LearnerId == learnerId).Select(m => m.TopicId).ToListAsync(ct);
        var nextLesson = await _db.Lessons
            .Include(l => l.Topic).ThenInclude(t => t.Subject)
            .Where(l => l.Topic.GradeLevel.Grade == learner.GradeLevel && !startedTopics.Contains(l.TopicId) && l.IsActive && l.IsPublished)
            .OrderBy(l => l.Topic.SortOrder).ThenBy(l => l.SortOrder)
            .FirstOrDefaultAsync(ct);

        return new NextRecommendationDto
        {
            LessonId = nextLesson?.Id,
            TopicId = nextLesson?.TopicId,
            Title = nextLesson?.Title ?? "Explore a Subject!",
            Type = RecommendationType.NewLesson,
            SubjectName = nextLesson?.Topic?.Subject?.Name,
            ColourHex = nextLesson?.Topic?.Subject?.ColourHex
        };
    }

    public async Task<SessionResultDto> StartLearningSessionAsync(Guid learnerId, Guid topicId, CancellationToken ct = default)
    {
        var session = new LearningSession
        {
            LearnerId = learnerId,
            TopicId = topicId,
            Status = SessionStatus.Active,
            StartedAt = DateTime.UtcNow
        };
        _db.LearningSessions.Add(session);
        await _db.SaveChangesAsync(ct);
        return new SessionResultDto { SessionId = session.Id, Started = true };
    }

    public async Task<bool> CompleteLearningSessionAsync(Guid sessionId, int pointsEarned, CancellationToken ct = default)
    {
        var session = await _db.LearningSessions.FindAsync(new object[] { sessionId }, ct);
        if (session == null) return false;
        session.Status = SessionStatus.Completed;
        session.EndedAt = DateTime.UtcNow;
        session.DurationMinutes = (int)(session.EndedAt.Value - session.StartedAt).TotalMinutes;
        session.PointsEarned = pointsEarned;
        session.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
