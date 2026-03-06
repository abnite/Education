using EduKidsGhana.Application.DTOs.Gamification;

namespace EduKidsGhana.Application.Interfaces;

public interface IAchievementService
{
    Task<List<AchievementDto>> GetLearnerAchievementsAsync(Guid learnerId, CancellationToken ct = default);
    Task CheckAndAwardAchievementsAsync(Guid learnerId, CancellationToken ct = default);
    Task<List<AchievementDto>> GetNewlyUnlockedAsync(Guid learnerId, CancellationToken ct = default);
}
