using EduKidsGhana.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.AI;

/// <summary>
/// Fallback AI provider used when no AI is configured. Returns null for all operations
/// so the system can fall back to rule-based generation.
/// </summary>
public class DummyAIProvider : IAIProvider
{
    private readonly ILogger<DummyAIProvider> _logger;

    public DummyAIProvider(ILogger<DummyAIProvider> logger)
    {
        _logger = logger;
    }

    public bool IsAvailable => false;
    public string ProviderName => "None";

    public Task<string?> GenerateTextAsync(string prompt, int maxTokens = 500, CancellationToken ct = default)
    {
        _logger.LogDebug("DummyAIProvider: GenerateText called - returning null (AI disabled).");
        return Task.FromResult<string?>(null);
    }

    public Task<string?> GenerateExplanationAsync(string topic, string content, int gradeLevel, CancellationToken ct = default)
        => Task.FromResult<string?>(null);

    public Task<List<string>> GenerateQuizQuestionsAsync(string topic, string subject, int gradeLevel, int count, string difficulty, CancellationToken ct = default)
        => Task.FromResult(new List<string>());

    public Task<string?> GenerateHintAsync(string questionText, string correctAnswer, CancellationToken ct = default)
        => Task.FromResult<string?>(null);

    public Task<string?> SimplifyContentAsync(string content, int targetAge, CancellationToken ct = default)
        => Task.FromResult<string?>(null);
}
