using EduKidsGhana.Application.DTOs.Curriculum;

namespace EduKidsGhana.Application.Interfaces;

public interface ILessonService
{
    Task<LessonDetailDto?> GetLessonAsync(Guid lessonId, CancellationToken ct = default);
    Task<List<LessonSummaryDto>> GetLessonsByTopicAsync(Guid topicId, CancellationToken ct = default);
    Task<LessonDetailDto> CreateLessonAsync(CreateLessonDto dto, CancellationToken ct = default);
    Task<LessonDetailDto> UpdateLessonAsync(Guid lessonId, CreateLessonDto dto, CancellationToken ct = default);
    Task<bool> PublishLessonAsync(Guid lessonId, CancellationToken ct = default);
    Task<bool> DeleteLessonAsync(Guid lessonId, CancellationToken ct = default);
}
