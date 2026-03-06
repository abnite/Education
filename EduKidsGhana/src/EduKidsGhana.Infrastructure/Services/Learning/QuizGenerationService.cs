using EduKidsGhana.Application.DTOs.Quiz;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Constants;
using EduKidsGhana.Domain.Entities.Assessment;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using EduKidsGhana.Infrastructure.Services.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Learning;

public class QuizGenerationService : IQuizGenerationService
{
    private readonly AppDbContext _db;
    private readonly IRuleBasedQuestionGenerator _ruleGenerator;
    private readonly AIProviderFactory _aiFactory;
    private readonly IAdaptiveLearningService _adaptiveService;
    private readonly IRewardService _rewardService;
    private readonly ILogger<QuizGenerationService> _logger;

    public QuizGenerationService(
        AppDbContext db,
        IRuleBasedQuestionGenerator ruleGenerator,
        AIProviderFactory aiFactory,
        IAdaptiveLearningService adaptiveService,
        IRewardService rewardService,
        ILogger<QuizGenerationService> logger)
    {
        _db = db;
        _ruleGenerator = ruleGenerator;
        _aiFactory = aiFactory;
        _adaptiveService = adaptiveService;
        _rewardService = rewardService;
        _logger = logger;
    }

    public async Task<GeneratedQuizDto> GenerateQuizAsync(GenerateQuizRequestDto request, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FirstOrDefaultAsync(l => l.Id == request.LearnerId, ct)
            ?? throw new InvalidOperationException("Learner not found.");
        var topic = await _db.Topics.Include(t => t.Subject).FirstOrDefaultAsync(t => t.Id == request.TopicId, ct)
            ?? throw new InvalidOperationException("Topic not found.");

        var difficulty = request.Difficulty
            ?? await _adaptiveService.DetermineNextDifficultyAsync(request.LearnerId, request.TopicId, ct);

        List<QuizQuestionDto> questions;
        var sourceType = GenerationSourceType.RuleBased;

        // Try AI first if requested
        if (request.UseAI)
        {
            var aiProvider = await _aiFactory.GetProviderAsync(ct);
            if (aiProvider.IsAvailable)
            {
                // AI generation - in production this would parse AI responses into questions
                // For now, fall back to rule-based with AI explanation enhancement
                sourceType = GenerationSourceType.RuleBased;
                _logger.LogInformation("AI available but using rule-based for quiz structure reliability.");
            }
        }

        // Rule-based generation (guaranteed to work)
        questions = topic.Subject.Code switch
        {
            "MATHS" => await _ruleGenerator.GenerateMathsQuestionsAsync(learner.GradeLevel, difficulty, request.QuestionCount, ct),
            "ENGLISH" => await _ruleGenerator.GenerateEnglishQuestionsAsync(learner.GradeLevel, difficulty, request.QuestionCount, ct),
            "SCIENCE" => await _ruleGenerator.GenerateScienceQuestionsAsync(request.TopicId, learner.GradeLevel, difficulty, request.QuestionCount, ct),
            "PROG" => await _ruleGenerator.GenerateProgrammingQuestionsAsync(learner.GradeLevel, difficulty, request.QuestionCount, ct),
            _ => await _ruleGenerator.GenerateMathsQuestionsAsync(learner.GradeLevel, difficulty, request.QuestionCount, ct)
        };

        // Persist the quiz
        var quiz = new GeneratedQuiz
        {
            LearnerId = request.LearnerId,
            TopicId = request.TopicId,
            Title = $"{topic.Name} Quiz",
            SourceType = sourceType,
            Difficulty = difficulty,
            TimeLimitSeconds = DomainConstants.Quiz.DefaultTimeLimitSeconds,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
        _db.GeneratedQuizzes.Add(quiz);
        await _db.SaveChangesAsync(ct);

        return new GeneratedQuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            TopicId = topic.Id,
            TopicName = topic.Name,
            Difficulty = difficulty,
            TimeLimitSeconds = quiz.TimeLimitSeconds,
            SourceType = sourceType,
            Questions = questions
        };
    }

    public async Task<QuizSubmitResultDto> SubmitQuizAsync(QuizSubmissionDto submission, CancellationToken ct = default)
    {
        var quiz = await _db.GeneratedQuizzes
            .Include(q => q.Questions).ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == submission.QuizId, ct)
            ?? throw new InvalidOperationException("Quiz not found.");

        int correct = 0, total = quiz.Questions.Count, totalPts = total * 10;

        var attempt = new QuizAttempt
        {
            QuizId = quiz.Id,
            LearnerId = submission.LearnerId,
            TotalQuestions = total,
            TotalPoints = totalPts,
            DurationSeconds = submission.DurationSeconds,
            StartedAt = DateTime.UtcNow.AddSeconds(-submission.DurationSeconds),
            CompletedAt = DateTime.UtcNow
        };
        _db.QuizAttempts.Add(attempt);
        await _db.SaveChangesAsync(ct);

        foreach (var answer in submission.Answers)
        {
            var question = quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question == null) continue;
            bool isCorrect = string.Equals(answer.AnswerGiven.Trim(), question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
            if (isCorrect) correct++;
            var learnerAnswer = new LearnerAnswer
            {
                AttemptId = attempt.Id,
                QuestionId = answer.QuestionId,
                AnswerGiven = answer.AnswerGiven,
                IsCorrect = isCorrect,
                PointsEarned = isCorrect ? question.Points : 0,
                TimeTakenSeconds = answer.TimeTakenSeconds,
                HintUsed = answer.HintUsed
            };
            _db.LearnerAnswers.Add(learnerAnswer);
        }

        double pct = total > 0 ? (double)correct / total * 100 : 0;
        bool passed = pct >= DomainConstants.Quiz.DefaultPassMarkPercent;
        int pts = passed ? DomainConstants.Gamification.PointsPerQuizPass : 0;
        if (pct == 100) pts = DomainConstants.Gamification.PointsPerPerfectQuiz;

        attempt.Score = correct;
        attempt.CorrectAnswers = correct;
        attempt.IsPassed = passed;
        await _db.SaveChangesAsync(ct);

        // Update mastery
        await _adaptiveService.UpdateMasteryAsync(submission.LearnerId, quiz.TopicId, pct, ct);
        if (!passed)
            await _adaptiveService.QueueForRevisionAsync(submission.LearnerId, quiz.TopicId, ct: ct);

        // Award points
        if (pts > 0)
            await _rewardService.AwardPointsAsync(submission.LearnerId, pts, RewardType.QuizCompletion, $"Quiz completed: {quiz.Title}", quiz.Id.ToString(), ct);

        string msg = pct >= 90 ? "Excellent work! You are a star!" :
                     pct >= 70 ? "Well done! Keep practising!" :
                     pct >= 50 ? "Good try! Review and try again." :
                     "Keep learning! You can do it!";

        return new QuizSubmitResultDto
        {
            AttemptId = attempt.Id,
            Score = correct,
            TotalPoints = totalPts,
            CorrectAnswers = correct,
            TotalQuestions = total,
            Percentage = pct,
            IsPassed = passed,
            PointsEarned = pts,
            EncouragementMessage = msg
        };
    }

    public async Task<QuizReviewDto> GetQuizReviewAsync(Guid attemptId, CancellationToken ct = default)
    {
        var attempt = await _db.QuizAttempts
            .Include(a => a.Answers).ThenInclude(a => a.Question).ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == attemptId, ct)
            ?? throw new InvalidOperationException("Attempt not found.");

        return new QuizReviewDto
        {
            AttemptId = attempt.Id,
            Score = attempt.Score,
            Percentage = attempt.TotalQuestions > 0 ? (double)attempt.CorrectAnswers / attempt.TotalQuestions * 100 : 0,
            IsPassed = attempt.IsPassed,
            AnswerReviews = attempt.Answers.Select(a => new QuizAnswerReviewDto
            {
                QuestionText = a.Question.QuestionText,
                AnswerGiven = a.AnswerGiven,
                CorrectAnswer = a.Question.CorrectAnswer,
                IsCorrect = a.IsCorrect,
                Explanation = a.Question.Explanation,
                PointsEarned = a.PointsEarned
            }).ToList()
        };
    }

    public async Task<HintResponseDto> GetHintAsync(HintRequestDto request, CancellationToken ct = default)
    {
        var question = await _db.GeneratedQuizQuestions
            .Include(q => q.Quiz).ThenInclude(q => q.Topic).ThenInclude(t => t.Subject)
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, ct);

        if (question?.HintText != null)
            return new HintResponseDto { HintText = question.HintText, SourceType = "Seeded" };

        // Try AI hint
        var aiProvider = await _aiFactory.GetProviderAsync(ct);
        if (aiProvider.IsAvailable && question != null)
        {
            var aiHint = await aiProvider.GenerateHintAsync(question.QuestionText, question.CorrectAnswer, ct);
            if (aiHint != null)
                return new HintResponseDto { HintText = aiHint, SourceType = "AI" };
        }

        // Fallback to rule-based
        var subjectCode = question?.Quiz?.Topic?.Subject?.Code ?? "MATHS";
        var hint = await _ruleGenerator.GenerateFallbackHintAsync(question?.QuestionText ?? "", subjectCode, ct);
        return new HintResponseDto { HintText = hint, SourceType = "Rule" };
    }
}
