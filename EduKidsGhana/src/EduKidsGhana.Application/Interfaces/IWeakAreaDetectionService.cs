using EduKidsGhana.Application.DTOs.Learning;

namespace EduKidsGhana.Application.Interfaces;

public interface IWeakAreaDetectionService
{
    Task<List<WeakAreaDto>> DetectWeakAreasAsync(Guid learnerId, CancellationToken ct = default);
    Task UpdateWeakAreasAsync(Guid learnerId, Guid topicId, double score, CancellationToken ct = default);
    Task<bool> IsWeakAreaAsync(Guid learnerId, Guid topicId, CancellationToken ct = default);
}
