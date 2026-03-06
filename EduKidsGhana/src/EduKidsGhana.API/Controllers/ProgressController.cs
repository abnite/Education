using EduKidsGhana.Application.DTOs.Gamification;
using EduKidsGhana.Application.DTOs.Progress;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressTrackingService _progressService;
    private readonly IAchievementService _achievementService;

    public ProgressController(IProgressTrackingService progressService, IAchievementService achievementService)
    {
        _progressService = progressService;
        _achievementService = achievementService;
    }

    [HttpGet("{learnerId:guid}/summary")]
    public async Task<ActionResult<ApiResponse<LearnerProgressSummaryDto>>> GetSummary(Guid learnerId, CancellationToken ct) =>
        Ok(ApiResponse<LearnerProgressSummaryDto>.Ok(await _progressService.GetProgressSummaryAsync(learnerId, ct)));

    [HttpGet("{learnerId:guid}/masteries")]
    public async Task<ActionResult<ApiResponse<List<TopicMasteryDto>>>> GetMasteries(Guid learnerId, CancellationToken ct) =>
        Ok(ApiResponse<List<TopicMasteryDto>>.Ok(await _progressService.GetTopicMasteriesAsync(learnerId, ct)));

    [HttpGet("{learnerId:guid}/achievements")]
    public async Task<ActionResult<ApiResponse<List<AchievementDto>>>> GetAchievements(Guid learnerId, CancellationToken ct) =>
        Ok(ApiResponse<List<AchievementDto>>.Ok(await _achievementService.GetLearnerAchievementsAsync(learnerId, ct)));

    [HttpPost("{learnerId:guid}/lesson-complete/{lessonId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> RecordLessonComplete(Guid learnerId, Guid lessonId, [FromQuery] int minutes = 15, CancellationToken ct = default)
    {
        await _progressService.RecordLessonCompletionAsync(learnerId, lessonId, minutes, ct);
        await _achievementService.CheckAndAwardAchievementsAsync(learnerId, ct);
        return Ok(ApiResponse<object>.Ok("Progress recorded!"));
    }

    [HttpPost("{learnerId:guid}/streak")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStreak(Guid learnerId, CancellationToken ct)
    {
        await _progressService.UpdateStreakAsync(learnerId, ct);
        return Ok(ApiResponse<object>.Ok("Streak updated!"));
    }
}
