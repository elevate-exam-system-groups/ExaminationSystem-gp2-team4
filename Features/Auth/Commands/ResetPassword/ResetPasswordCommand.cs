using Examination_System.Common.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ApiResponse<string>>
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}