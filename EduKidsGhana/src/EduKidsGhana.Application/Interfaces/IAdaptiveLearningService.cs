using EduKidsGhana.Application.DTOs.Learning;
using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.Interfaces;

public interface IAdaptiveLearningService
{
    Task<DifficultyLevel> DetermineNextDifficultyAsync(Guid learnerId, Guid topicId, CancellationToken ct = default);
    Task UpdateMasteryAsync(Guid learnerId, Guid topicId, double scorePercent, CancellationToken ct = default);
    Task<MasteryLevel> GetCurrentMasteryLevelAsync(Guid learnerId, Guid topicId, CancellationToken ct = default);
    Task QueueForRevisionAsync(Guid learnerId, Guid topicId, Guid? questionId = null, CancellationToken ct = default);
    Task<List<RevisionItemDto>> GetRevisionQueueAsync(Guid learnerId, CancellationToken ct = default);
    Task<bool> IsReadyForProgressionAsync(Guid learnerId, Guid topicId, CancellationToken ct = default);
}
