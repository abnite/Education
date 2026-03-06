using EduKidsGhana.Application.DTOs.Learner;

namespace EduKidsGhana.Application.Interfaces;

public interface ILearnerService
{
    Task<LearnerProfileDto?> GetProfileAsync(Guid learnerId, CancellationToken ct = default);
    Task<LearnerProfileDto> CreateLearnerAsync(CreateLearnerDto dto, Guid parentUserId, CancellationToken ct = default);
    Task<LearnerProfileDto> UpdateProfileAsync(Guid learnerId, UpdateLearnerDto dto, CancellationToken ct = default);
    Task<LearnerDashboardDto> GetDashboardAsync(Guid learnerId, CancellationToken ct = default);
    Task<bool> SetAvatarAsync(Guid learnerId, string avatarCode, CancellationToken ct = default);
    Task<List<AvatarDto>> GetAvailableAvatarsAsync(CancellationToken ct = default);
}
