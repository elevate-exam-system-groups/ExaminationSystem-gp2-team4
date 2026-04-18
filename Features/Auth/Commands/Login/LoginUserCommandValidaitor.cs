using FluentValidation;

namespace Examination_System.Features.Auth.Commands.Login
{
    public class LoginUserCommandValidaitor : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidaitor()
        {
            RuleFor(x => x.email)
                 .NotEmpty().WithMessage("Email Is Required")
                .EmailAddress().WithMessage("Invalid Email Format");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password Is Required")
                .MinimumLength(6).WithMessage("Password Must Be At Least 6 Characters")
                .Matches(@"[A-Z]").WithMessage("Password Must Contain at Least One Uppercase Letter")
                .Matches(@"[a-z]").WithMessage("Password Must Contain at Least One Lowercase Letter")
                .Matches(@"\d").WithMessage("Password Must Contain at Least One Number");

        }

    }
}