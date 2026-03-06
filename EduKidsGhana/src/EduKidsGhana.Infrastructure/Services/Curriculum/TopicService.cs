using AutoMapper;
using EduKidsGhana.Application.DTOs.Curriculum;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.Curriculum;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduKidsGhana.Infrastructure.Services.Curriculum;

public class TopicService : ITopicService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public TopicService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<List<TopicDto>> GetTopicsBySubjectAndGradeAsync(Guid subjectId, int gradeLevel, CancellationToken ct = default) =>
        _mapper.Map<List<TopicDto>>(await _db.Topics
            .Include(t => t.GradeLevel)
            .Where(t => t.SubjectId == subjectId && t.GradeLevel.Grade == gradeLevel && t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(ct));

    public async Task<TopicDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var t = await _db.Topics.FindAsync(new object[] { id }, ct);
        return t == null ? null : _mapper.Map<TopicDto>(t);
    }

    public async Task<TopicDto> CreateAsync(CreateTopicDto dto, CancellationToken ct = default)
    {
        var topic = _mapper.Map<Topic>(dto);
        _db.Topics.Add(topic);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<TopicDto>(topic);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var topic = await _db.Topics.FindAsync(new object[] { id }, ct);
        if (topic == null) return false;
        topic.IsDeleted = true;
        topic.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
