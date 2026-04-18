using Examination_System.Features.Auth.Commands.Registeration;
using FluentValidation;

namespace Examination_System.Features.Auth.Commands.Registeration
{
    public class RegisterUserCommandValidaitor : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidaitor()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName Is Required")
                .MaximumLength(100).WithMessage("FullName Must Not Exceed 100 Characters");


            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email Is Required")
                .EmailAddress().WithMessage("Invalid Email Format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password Is Required")
                .MinimumLength(6).WithMessage("Password Must Be At Least 6 Characters")
                .Matches(@"[A-Z]").WithMessage("Password Must Contain at Least One Uppercase Letter")
                .Matches(@"[a-z]").WithMessage("Password Must Contain at Least One Lowercase Letter")
                .Matches(@"\d").WithMessage("Password Must Contain at Least One Number");







        }
    }
}
