namespace EduKidsGhana.Application.Interfaces;

public interface IAIProvider
{
    bool IsAvailable { get; }
    string ProviderName { get; }
    Task<string?> GenerateTextAsync(string prompt, int maxTokens = 500, CancellationToken ct = default);
    Task<string?> GenerateExplanationAsync(string topic, string content, int gradeLevel, CancellationToken ct = default);
    Task<List<string>> GenerateQuizQuestionsAsync(string topic, string subject, int gradeLevel, int count, string difficulty, CancellationToken ct = default);
    Task<string?> GenerateHintAsync(string questionText, string correctAnswer, CancellationToken ct = default);
    Task<string?> SimplifyContentAsync(string content, int targetAge, CancellationToken ct = default);
}
