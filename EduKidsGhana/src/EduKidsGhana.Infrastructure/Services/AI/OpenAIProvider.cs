using System.Net.Http.Json;
using System.Text.Json;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.AI;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.AI;

public class OpenAIProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly AIProviderSetting _setting;
    private readonly ILogger<OpenAIProvider> _logger;

    public OpenAIProvider(HttpClient httpClient, AIProviderSetting setting, ILogger<OpenAIProvider> logger)
    {
        _httpClient = httpClient;
        _setting = setting;
        _logger = logger;
    }

    public bool IsAvailable => _setting.IsEnabled && !string.IsNullOrEmpty(_setting.ApiKey);
    public string ProviderName => "OpenAI";

    public async Task<string?> GenerateTextAsync(string prompt, int maxTokens = 500, CancellationToken ct = default)
    {
        if (!IsAvailable) return null;
        try
        {
            var endpoint = _setting.ApiEndpoint ?? "https://api.openai.com/v1/chat/completions";
            var model = _setting.ModelName ?? "gpt-4o-mini";
            var request = new
            {
                model,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = maxTokens,
                temperature = _setting.Temperature
            };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _setting.ApiKey);
            httpRequest.Content = JsonContent.Create(request);
            var response = await _httpClient.SendAsync(httpRequest, ct);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenAI call failed. Falling back to rule-based.");
            return null;
        }
    }

    public async Task<string?> GenerateExplanationAsync(string topic, string content, int gradeLevel, CancellationToken ct = default)
    {
        var prompt = $"You are a friendly tutor for Ghanaian children in Grade {gradeLevel}. " +
                     $"Explain the following topic about '{topic}' in simple, engaging language suitable for a {gradeLevel + 5} year old child in Ghana. " +
                     $"Use examples with Ghanaian context where possible (e.g. mangoes, cedis, Accra). " +
                     $"Keep it short, clear and encouraging.\n\nContent: {content}";
        return await GenerateTextAsync(prompt, _setting.MaxTokens, ct);
    }

    public async Task<List<string>> GenerateQuizQuestionsAsync(string topic, string subject, int gradeLevel, int count, string difficulty, CancellationToken ct = default)
    {
        var prompt = $"Generate {count} multiple-choice quiz questions for a Grade {gradeLevel} Ghanaian child on the topic '{topic}' (subject: {subject}), difficulty: {difficulty}. " +
                     $"Format each question as JSON: {{\"question\": \"...\", \"options\": [\"A. ...\", \"B. ...\", \"C. ...\", \"D. ...\"], \"answer\": \"A\"}}. " +
                     $"Use Ghana-relevant examples where possible. Return only a JSON array.";
        var result = await GenerateTextAsync(prompt, 1000, ct);
        if (string.IsNullOrEmpty(result)) return new List<string>();
        try
        {
            var questions = JsonSerializer.Deserialize<List<string>>(result);
            return questions ?? new List<string>();
        }
        catch
        {
            return new List<string> { result };
        }
    }

    public async Task<string?> GenerateHintAsync(string questionText, string correctAnswer, CancellationToken ct = default)
    {
        var prompt = $"Give a short, friendly hint for a Ghanaian child to help them answer this question WITHOUT revealing the answer: '{questionText}'. Answer is '{correctAnswer}'. Keep it to 1-2 sentences.";
        return await GenerateTextAsync(prompt, 150, ct);
    }

    public async Task<string?> SimplifyContentAsync(string content, int targetAge, CancellationToken ct = default)
    {
        var prompt = $"Simplify the following educational content for a {targetAge}-year-old child in Ghana. Use simple words and short sentences:\n\n{content}";
        return await GenerateTextAsync(prompt, 300, ct);
    }
}
