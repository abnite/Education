using FluentValidation;
using EduKidsGhana.Application.DTOs.Learner;

namespace EduKidsGhana.Application.Validators;

public class CreateLearnerValidator : AbstractValidator<CreateLearnerDto>
{
    public CreateLearnerValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100).WithMessage("Display name is required.");
        RuleFor(x => x.Age).InclusiveBetween(4, 14).WithMessage("Age must be between 4 and 14.");
        RuleFor(x => x.GradeLevel).InclusiveBetween(1, 6).WithMessage("Grade level must be between 1 and 6.");
    }
}

public class UpdateLearnerValidator : AbstractValidator<UpdateLearnerDto>
{
    public UpdateLearnerValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Age).InclusiveBetween(4, 14);
        RuleFor(x => x.GradeLevel).InclusiveBetween(1, 6);
    }
}
