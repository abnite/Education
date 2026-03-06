using EduKidsGhana.Application.DTOs.Quiz;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizController : ControllerBase
{
    private readonly IQuizGenerationService _quizService;

    public QuizController(IQuizGenerationService quizService) => _quizService = quizService;

    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<GeneratedQuizDto>>> Generate([FromBody] GenerateQuizRequestDto dto, CancellationToken ct)
    {
        var quiz = await _quizService.GenerateQuizAsync(dto, ct);
        return Ok(ApiResponse<GeneratedQuizDto>.Ok(quiz, "Quiz ready! Good luck!"));
    }

    [HttpPost("submit")]
    public async Task<ActionResult<ApiResponse<QuizSubmitResultDto>>> Submit([FromBody] QuizSubmissionDto dto, CancellationToken ct)
    {
        var result = await _quizService.SubmitQuizAsync(dto, ct);
        return Ok(ApiResponse<QuizSubmitResultDto>.Ok(result, result.EncouragementMessage));
    }

    [HttpGet("review/{attemptId:guid}")]
    public async Task<ActionResult<ApiResponse<QuizReviewDto>>> GetReview(Guid attemptId, CancellationToken ct)
    {
        var review = await _quizService.GetQuizReviewAsync(attemptId, ct);
        return Ok(ApiResponse<QuizReviewDto>.Ok(review));
    }

    [HttpPost("hint")]
    public async Task<ActionResult<ApiResponse<HintResponseDto>>> GetHint([FromBody] HintRequestDto dto, CancellationToken ct)
    {
        var hint = await _quizService.GetHintAsync(dto, ct);
        return Ok(ApiResponse<HintResponseDto>.Ok(hint));
    }
}
