using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.Interfaces;

public interface IRewardService
{
    Task AwardPointsAsync(Guid learnerId, int points, RewardType rewardType, string description, string? referenceId = null, CancellationToken ct = default);
    Task<int> GetTotalPointsAsync(Guid learnerId, CancellationToken ct = default);
}
