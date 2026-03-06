using AutoMapper;
using EduKidsGhana.Application.DTOs.Curriculum;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.Curriculum;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduKidsGhana.Infrastructure.Services.Curriculum;

public class SubjectService : ISubjectService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public SubjectService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<List<SubjectDto>> GetAllSubjectsAsync(CancellationToken ct = default) =>
        _mapper.Map<List<SubjectDto>>(await _db.Subjects.Where(s => s.IsActive).OrderBy(s => s.SortOrder).ToListAsync(ct));

    public async Task<SubjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _db.Subjects.FindAsync(new object[] { id }, ct);
        return s == null ? null : _mapper.Map<SubjectDto>(s);
    }

    public async Task<SubjectDto> CreateAsync(CreateSubjectDto dto, CancellationToken ct = default)
    {
        var subject = _mapper.Map<Subject>(dto);
        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task<SubjectDto> UpdateAsync(Guid id, CreateSubjectDto dto, CancellationToken ct = default)
    {
        var subject = await _db.Subjects.FindAsync(new object[] { id }, ct)
            ?? throw new InvalidOperationException("Subject not found.");
        _mapper.Map(dto, subject);
        subject.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var subject = await _db.Subjects.FindAsync(new object[] { id }, ct);
        if (subject == null) return false;
        subject.IsDeleted = true;
        subject.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
