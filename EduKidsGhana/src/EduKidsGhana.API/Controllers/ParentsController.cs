using EduKidsGhana.Application.DTOs.Parent;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Parent,Admin")]
public class ParentsController : ControllerBase
{
    private readonly IParentService _parentService;

    public ParentsController(IParentService parentService) => _parentService = parentService;

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<ParentDashboardDto>>> GetDashboard(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var dashboard = await _parentService.GetDashboardAsync(userId.Value, ct);
        return Ok(ApiResponse<ParentDashboardDto>.Ok(dashboard));
    }

    [HttpPost("link-child")]
    public async Task<ActionResult<ApiResponse<object>>> LinkChild([FromBody] LinkChildDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var success = await _parentService.LinkChildAsync(userId.Value, dto.LearnerProfileId, ct);
        if (!success) return BadRequest(ApiResponse<object>.Fail("Could not link child."));
        return Ok(ApiResponse<object>.Ok("Child linked successfully!"));
    }

    [HttpGet("children")]
    public async Task<ActionResult<ApiResponse<List<ChildProgressDto>>>> GetChildrenProgress(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var progress = await _parentService.GetChildrenProgressAsync(userId.Value, ct);
        return Ok(ApiResponse<List<ChildProgressDto>>.Ok(progress));
    }

    [HttpGet("children/{learnerId:guid}/progress")]
    public async Task<ActionResult<ApiResponse<ChildProgressDto>>> GetChildProgress(Guid learnerId, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var progress = await _parentService.GetChildProgressDetailAsync(userId.Value, learnerId, ct);
        return Ok(ApiResponse<ChildProgressDto>.Ok(progress));
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
