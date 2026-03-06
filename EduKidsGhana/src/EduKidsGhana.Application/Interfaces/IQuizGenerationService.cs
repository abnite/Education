using EduKidsGhana.Application.DTOs.Quiz;

namespace EduKidsGhana.Application.Interfaces;

public interface IQuizGenerationService
{
    Task<GeneratedQuizDto> GenerateQuizAsync(GenerateQuizRequestDto request, CancellationToken ct = default);
    Task<QuizReviewDto> GetQuizReviewAsync(Guid attemptId, CancellationToken ct = default);
    Task<QuizSubmitResultDto> SubmitQuizAsync(QuizSubmissionDto submission, CancellationToken ct = default);
    Task<HintResponseDto> GetHintAsync(HintRequestDto request, CancellationToken ct = default);
}
