using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.Gamification;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Gamification;

public class RewardService : IRewardService
{
    private readonly AppDbContext _db;
    private readonly ILogger<RewardService> _logger;

    public RewardService(AppDbContext db, ILogger<RewardService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task AwardPointsAsync(Guid learnerId, int points, RewardType rewardType, string description, string? referenceId = null, CancellationToken ct = default)
    {
        if (points <= 0) return;

        _db.RewardTransactions.Add(new RewardTransaction
        {
            LearnerId = learnerId,
            Points = points,
            RewardType = rewardType,
            Description = description,
            ReferenceId = referenceId
        });

        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        if (learner != null)
        {
            learner.TotalPoints += points;
            learner.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Awarded {Points} points to learner {LearnerId} for {Reason}", points, learnerId, description);
    }

    public async Task<int> GetTotalPointsAsync(Guid learnerId, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        return learner?.TotalPoints ?? 0;
    }
}
