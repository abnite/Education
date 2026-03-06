using EduKidsGhana.Application.DTOs.Learning;

namespace EduKidsGhana.Application.Interfaces;

public interface ILessonRecommendationService
{
    Task<List<RecommendationDto>> GetRecommendationsAsync(Guid learnerId, int count = 3, CancellationToken ct = default);
    Task<RecommendationDto?> GetPriorityRecommendationAsync(Guid learnerId, CancellationToken ct = default);
    Task GenerateRecommendationsAsync(Guid learnerId, CancellationToken ct = default);
}
