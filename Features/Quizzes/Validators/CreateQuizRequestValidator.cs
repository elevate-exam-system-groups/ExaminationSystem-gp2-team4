using Examination_System.Features.Quizzes.DTOs;
using FluentValidation;

namespace Examination_System.Features.Quizzes.Validators;

public class CreateQuizRequestValidator : AbstractValidator<CreateQuizRequst>
{
    public CreateQuizRequestValidator()
    {
        RuleFor(x => x.DiplomaId)
            .NotEmpty()
            .WithMessage("DiplomaId is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0")
            .LessThanOrEqualTo(480)
            .WithMessage("Duration cannot exceed 480 minutes (8 hours)");

        RuleFor(x => x.PassScore)
            .InclusiveBetween(0, 100)
            .WithMessage("PassScore must be between 0 and 100");

        RuleFor(x => x.MaxAttempts)
            .InclusiveBetween(1, 10)
            .WithMessage("MaxAttempts must be between 1 and 10");
    }
}