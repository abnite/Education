using EduKidsGhana.Application.DTOs.Curriculum;

namespace EduKidsGhana.Application.Interfaces;

public interface ITopicService
{
    Task<List<TopicDto>> GetTopicsBySubjectAndGradeAsync(Guid subjectId, int gradeLevel, CancellationToken ct = default);
    Task<TopicDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TopicDto> CreateAsync(CreateTopicDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
