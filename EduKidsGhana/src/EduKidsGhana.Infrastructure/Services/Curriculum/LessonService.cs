using AutoMapper;
using EduKidsGhana.Application.DTOs.Curriculum;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.Curriculum;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduKidsGhana.Infrastructure.Services.Curriculum;

public class LessonService : ILessonService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public LessonService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<LessonDetailDto?> GetLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons
            .Include(l => l.Topic).ThenInclude(t => t.Subject)
            .Include(l => l.Sections)
            .Include(l => l.Objectives)
            .Include(l => l.Examples)
            .Include(l => l.AudioFiles)
            .FirstOrDefaultAsync(l => l.Id == lessonId, ct);
        return lesson == null ? null : _mapper.Map<LessonDetailDto>(lesson);
    }

    public async Task<List<LessonSummaryDto>> GetLessonsByTopicAsync(Guid topicId, CancellationToken ct = default) =>
        _mapper.Map<List<LessonSummaryDto>>(await _db.Lessons.Where(l => l.TopicId == topicId && l.IsActive && l.IsPublished).OrderBy(l => l.SortOrder).ToListAsync(ct));

    public async Task<LessonDetailDto> CreateLessonAsync(CreateLessonDto dto, CancellationToken ct = default)
    {
        var lesson = _mapper.Map<Lesson>(dto);
        _db.Lessons.Add(lesson);

        foreach (var (objText, idx) in dto.Objectives.Select((o, i) => (o, i)))
            _db.LessonObjectives.Add(new LessonObjective { LessonId = lesson.Id, Description = objText, SortOrder = idx });
        foreach (var sec in dto.Sections)
            _db.LessonSections.Add(new LessonSection { LessonId = lesson.Id, Title = sec.Title, Content = sec.Content, ContentHtml = sec.ContentHtml, SortOrder = sec.SortOrder, AudioUrl = sec.AudioUrl });
        foreach (var ex in dto.Examples)
            _db.LessonExamples.Add(new LessonExample { LessonId = lesson.Id, Title = ex.Title, Content = ex.Content, Solution = ex.Solution, Explanation = ex.Explanation, SortOrder = ex.SortOrder });

        await _db.SaveChangesAsync(ct);
        return await GetLessonAsync(lesson.Id, ct) ?? throw new InvalidOperationException("Failed to retrieve created lesson.");
    }

    public async Task<LessonDetailDto> UpdateLessonAsync(Guid lessonId, CreateLessonDto dto, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons.FindAsync(new object[] { lessonId }, ct)
            ?? throw new InvalidOperationException("Lesson not found.");
        lesson.Title = dto.Title;
        lesson.Summary = dto.Summary;
        lesson.LessonType = dto.LessonType;
        lesson.Difficulty = dto.Difficulty;
        lesson.EstimatedMinutes = dto.EstimatedMinutes;
        lesson.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return await GetLessonAsync(lessonId, ct) ?? throw new InvalidOperationException("Lesson not found.");
    }

    public async Task<bool> PublishLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons.FindAsync(new object[] { lessonId }, ct);
        if (lesson == null) return false;
        lesson.IsPublished = true;
        lesson.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons.FindAsync(new object[] { lessonId }, ct);
        if (lesson == null) return false;
        lesson.IsDeleted = true;
        lesson.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
