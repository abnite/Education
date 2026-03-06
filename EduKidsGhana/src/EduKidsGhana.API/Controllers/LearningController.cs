using EduKidsGhana.Application.DTOs.Learning;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LearningController : ControllerBase
{
    private readonly ILearningOrchestrator _orchestrator;
    private readonly IAdaptiveLearningService _adaptiveService;
    private readonly ILessonRecommendationService _recommendationService;

    public LearningController(ILearningOrchestrator orchestrator, IAdaptiveLearningService adaptiveService, ILessonRecommendationService recommendationService)
    {
        _orchestrator = orchestrator;
        _adaptiveService = adaptiveService;
        _recommendationService = recommendationService;
    }

    [HttpGet("lesson-player/{learnerId:guid}/{lessonId:guid}")]
    public async Task<ActionResult<ApiResponse<LessonPlayerPayloadDto>>> GetLessonPlayer(Guid learnerId, Guid lessonId, CancellationToken ct)
    {
        var payload = await _orchestrator.GetLessonPlayerPayloadAsync(learnerId, lessonId, ct);
        return Ok(ApiResponse<LessonPlayerPayloadDto>.Ok(payload));
    }

    [HttpGet("recommendation/{learnerId:guid}")]
    public async Task<ActionResult<ApiResponse<NextRecommendationDto>>> GetNextRecommendation(Guid learnerId, CancellationToken ct)
    {
        var rec = await _orchestrator.GetNextRecommendationAsync(learnerId, ct);
        return Ok(ApiResponse<NextRecommendationDto>.Ok(rec));
    }

    [HttpGet("recommendations/{learnerId:guid}")]
    public async Task<ActionResult<ApiResponse<List<RecommendationDto>>>> GetRecommendations(Guid learnerId, [FromQuery] int count = 3, CancellationToken ct = default)
    {
        var recs = await _recommendationService.GetRecommendationsAsync(learnerId, count, ct);
        return Ok(ApiResponse<List<RecommendationDto>>.Ok(recs));
    }

    [HttpPost("session/start")]
    public async Task<ActionResult<ApiResponse<SessionResultDto>>> StartSession([FromBody] StartSessionDto dto, CancellationToken ct)
    {
        var result = await _orchestrator.StartLearningSessionAsync(dto.LearnerId, dto.TopicId, ct);
        return Ok(ApiResponse<SessionResultDto>.Ok(result));
    }

    [HttpPost("session/{sessionId:guid}/complete")]
    public async Task<ActionResult<ApiResponse<object>>> CompleteSession(Guid sessionId, [FromBody] CompleteSessionDto dto, CancellationToken ct)
    {
        await _orchestrator.CompleteLearningSessionAsync(sessionId, dto.PointsEarned, ct);
        return Ok(ApiResponse<object>.Ok("Session completed!"));
    }

    [HttpGet("revision/{learnerId:guid}")]
    public async Task<ActionResult<ApiResponse<List<RevisionItemDto>>>> GetRevisionQueue(Guid learnerId, CancellationToken ct)
    {
        var items = await _adaptiveService.GetRevisionQueueAsync(learnerId, ct);
        return Ok(ApiResponse<List<RevisionItemDto>>.Ok(items));
    }
}

public record StartSessionDto(Guid LearnerId, Guid TopicId);
public record CompleteSessionDto(int PointsEarned);
