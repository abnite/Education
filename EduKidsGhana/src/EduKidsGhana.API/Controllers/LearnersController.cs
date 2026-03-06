using EduKidsGhana.Application.DTOs.Learner;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LearnersController : ControllerBase
{
    private readonly ILearnerService _learnerService;

    public LearnersController(ILearnerService learnerService) => _learnerService = learnerService;

    [HttpGet("{learnerId:guid}")]
    public async Task<ActionResult<ApiResponse<LearnerProfileDto>>> GetProfile(Guid learnerId, CancellationToken ct)
    {
        var profile = await _learnerService.GetProfileAsync(learnerId, ct);
        if (profile == null) return NotFound(ApiResponse<LearnerProfileDto>.Fail("Learner not found."));
        return Ok(ApiResponse<LearnerProfileDto>.Ok(profile));
    }

    [HttpPost]
    [Authorize(Roles = "Parent,Admin")]
    public async Task<ActionResult<ApiResponse<LearnerProfileDto>>> CreateLearner([FromBody] CreateLearnerDto dto, CancellationToken ct)
    {
        var parentUserId = GetUserId();
        if (parentUserId == null) return Unauthorized();
        var learner = await _learnerService.CreateLearnerAsync(dto, parentUserId.Value, ct);
        return CreatedAtAction(nameof(GetProfile), new { learnerId = learner.Id }, ApiResponse<LearnerProfileDto>.Ok(learner, "Learner created successfully!"));
    }

    [HttpPut("{learnerId:guid}")]
    public async Task<ActionResult<ApiResponse<LearnerProfileDto>>> UpdateProfile(Guid learnerId, [FromBody] UpdateLearnerDto dto, CancellationToken ct)
    {
        var updated = await _learnerService.UpdateProfileAsync(learnerId, dto, ct);
        return Ok(ApiResponse<LearnerProfileDto>.Ok(updated));
    }

    [HttpGet("{learnerId:guid}/dashboard")]
    public async Task<ActionResult<ApiResponse<LearnerDashboardDto>>> GetDashboard(Guid learnerId, CancellationToken ct)
    {
        var dashboard = await _learnerService.GetDashboardAsync(learnerId, ct);
        return Ok(ApiResponse<LearnerDashboardDto>.Ok(dashboard));
    }

    [HttpPut("{learnerId:guid}/avatar")]
    public async Task<ActionResult<ApiResponse<object>>> SetAvatar(Guid learnerId, [FromBody] SetAvatarDto dto, CancellationToken ct)
    {
        var success = await _learnerService.SetAvatarAsync(learnerId, dto.AvatarCode, ct);
        if (!success) return NotFound(ApiResponse<object>.Fail("Learner not found."));
        return Ok(ApiResponse<object>.Ok("Avatar updated!"));
    }

    [HttpGet("avatars")]
    public async Task<ActionResult<ApiResponse<List<AvatarDto>>>> GetAvatars(CancellationToken ct)
    {
        var avatars = await _learnerService.GetAvailableAvatarsAsync(ct);
        return Ok(ApiResponse<List<AvatarDto>>.Ok(avatars));
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record SetAvatarDto(string AvatarCode);
