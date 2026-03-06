using EduKidsGhana.Application.DTOs.Learning;

namespace EduKidsGhana.Application.Interfaces;

public interface ILearningOrchestrator
{
    Task<LessonPlayerPayloadDto> GetLessonPlayerPayloadAsync(Guid learnerId, Guid lessonId, CancellationToken ct = default);
    Task<NextRecommendationDto> GetNextRecommendationAsync(Guid learnerId, CancellationToken ct = default);
    Task<SessionResultDto> StartLearningSessionAsync(Guid learnerId, Guid topicId, CancellationToken ct = default);
    Task<bool> CompleteLearningSessionAsync(Guid sessionId, int pointsEarned, CancellationToken ct = default);
}
