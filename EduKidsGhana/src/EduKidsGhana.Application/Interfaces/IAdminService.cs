using EduKidsGhana.Application.DTOs.Admin;

namespace EduKidsGhana.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken ct = default);
    Task<List<UserSummaryDto>> GetAllUsersAsync(CancellationToken ct = default);
    Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct = default);
    Task<bool> UpdateAISettingsAsync(AISettingsDto dto, CancellationToken ct = default);
    Task<AISettingsDto?> GetAISettingsAsync(CancellationToken ct = default);
    Task<AnalyticsReportDto> GetAnalyticsReportAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
