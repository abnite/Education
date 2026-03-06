using EduKidsGhana.Application.DTOs.Curriculum;

namespace EduKidsGhana.Application.Interfaces;

public interface ISubjectService
{
    Task<List<SubjectDto>> GetAllSubjectsAsync(CancellationToken ct = default);
    Task<SubjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SubjectDto> CreateAsync(CreateSubjectDto dto, CancellationToken ct = default);
    Task<SubjectDto> UpdateAsync(Guid id, CreateSubjectDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
