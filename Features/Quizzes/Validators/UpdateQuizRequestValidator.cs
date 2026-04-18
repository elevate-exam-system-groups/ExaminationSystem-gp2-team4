using Examination_System.Features.Quizzes.DTOs;
using FluentValidation;

namespace Examination_System.Features.Quizzes.Validators;

public class UpdateQuizRequestValidator : AbstractValidator<UpdateQuizRequest>
{
    public UpdateQuizRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Quiz ID is required");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0")
            .LessThanOrEqualTo(480)
            .WithMessage("Duration cannot exceed 480 minutes (8 hours)")
            .When(x => x.DurationMinutes.HasValue);

        RuleFor(x => x.PassScore)
            .InclusiveBetween(0, 100)
            .WithMessage("PassScore must be between 0 and 100")
            .When(x => x.PassScore.HasValue);

        RuleFor(x => x.MaxAttempts)
            .InclusiveBetween(1, 10)
            .WithMessage("MaxAttempts must be between 1 and 10")
            .When(x => x.MaxAttempts.HasValue);
    }
}