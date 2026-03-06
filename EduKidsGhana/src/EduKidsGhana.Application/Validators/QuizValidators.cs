using FluentValidation;
using EduKidsGhana.Application.DTOs.Quiz;

namespace EduKidsGhana.Application.Validators;

public class GenerateQuizRequestValidator : AbstractValidator<GenerateQuizRequestDto>
{
    public GenerateQuizRequestValidator()
    {
        RuleFor(x => x.LearnerId).NotEmpty();
        RuleFor(x => x.TopicId).NotEmpty();
        RuleFor(x => x.QuestionCount).InclusiveBetween(1, 20).WithMessage("Question count must be between 1 and 20.");
    }
}

public class QuizSubmissionValidator : AbstractValidator<QuizSubmissionDto>
{
    public QuizSubmissionValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.LearnerId).NotEmpty();
        RuleFor(x => x.Answers).NotEmpty().WithMessage("At least one answer is required.");
        RuleForEach(x => x.Answers).SetValidator(new AnswerSubmissionValidator());
    }
}

public class AnswerSubmissionValidator : AbstractValidator<AnswerSubmissionDto>
{
    public AnswerSubmissionValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.AnswerGiven).NotNull();
    }
}
