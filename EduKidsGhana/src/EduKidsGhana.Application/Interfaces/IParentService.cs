using EduKidsGhana.Application.DTOs.Parent;

namespace EduKidsGhana.Application.Interfaces;

public interface IParentService
{
    Task<ParentDashboardDto> GetDashboardAsync(Guid parentUserId, CancellationToken ct = default);
    Task<bool> LinkChildAsync(Guid parentUserId, Guid learnerProfileId, CancellationToken ct = default);
    Task<List<ChildProgressDto>> GetChildrenProgressAsync(Guid parentUserId, CancellationToken ct = default);
    Task<ChildProgressDto> GetChildProgressDetailAsync(Guid parentUserId, Guid learnerProfileId, CancellationToken ct = default);
}
