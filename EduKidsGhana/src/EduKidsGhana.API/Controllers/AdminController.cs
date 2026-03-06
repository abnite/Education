using EduKidsGhana.Application.DTOs.Admin;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService) => _adminService = adminService;

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<AdminDashboardDto>>> GetDashboard(CancellationToken ct) =>
        Ok(ApiResponse<AdminDashboardDto>.Ok(await _adminService.GetDashboardAsync(ct)));

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<List<UserSummaryDto>>>> GetUsers(CancellationToken ct) =>
        Ok(ApiResponse<List<UserSummaryDto>>.Ok(await _adminService.GetAllUsersAsync(ct)));

    [HttpPost("users/{userId:guid}/deactivate")]
    public async Task<ActionResult<ApiResponse<object>>> DeactivateUser(Guid userId, CancellationToken ct)
    {
        await _adminService.DeactivateUserAsync(userId, ct);
        return Ok(ApiResponse<object>.Ok("User deactivated."));
    }

    [HttpGet("ai-settings")]
    public async Task<ActionResult<ApiResponse<AISettingsDto>>> GetAISettings(CancellationToken ct)
    {
        var settings = await _adminService.GetAISettingsAsync(ct);
        return Ok(ApiResponse<AISettingsDto>.Ok(settings ?? new AISettingsDto()));
    }

    [HttpPut("ai-settings")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAISettings([FromBody] AISettingsDto dto, CancellationToken ct)
    {
        await _adminService.UpdateAISettingsAsync(dto, ct);
        return Ok(ApiResponse<object>.Ok("AI settings updated."));
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<ApiResponse<AnalyticsReportDto>>> GetAnalytics([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;
        var report = await _adminService.GetAnalyticsReportAsync(fromDate, toDate, ct);
        return Ok(ApiResponse<AnalyticsReportDto>.Ok(report));
    }
}
