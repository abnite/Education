using EduKidsGhana.Application.DTOs.Quiz;
using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.Interfaces;

public interface IRuleBasedQuestionGenerator
{
    Task<List<QuizQuestionDto>> GenerateMathsQuestionsAsync(int gradeLevel, DifficultyLevel difficulty, int count, CancellationToken ct = default);
    Task<List<QuizQuestionDto>> GenerateEnglishQuestionsAsync(int gradeLevel, DifficultyLevel difficulty, int count, CancellationToken ct = default);
    Task<List<QuizQuestionDto>> GenerateScienceQuestionsAsync(Guid topicId, int gradeLevel, DifficultyLevel difficulty, int count, CancellationToken ct = default);
    Task<List<QuizQuestionDto>> GenerateProgrammingQuestionsAsync(int gradeLevel, DifficultyLevel difficulty, int count, CancellationToken ct = default);
    Task<string> GenerateFallbackHintAsync(string questionText, string subjectCode, CancellationToken ct = default);
}
