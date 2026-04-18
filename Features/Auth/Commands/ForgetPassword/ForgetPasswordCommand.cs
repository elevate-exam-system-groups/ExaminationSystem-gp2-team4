using Examination_System.Common.Wrappers;
using MediatR;

namespace Examination_System.Features.Auth.Commands.ForgetPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<ApiResponse<string>>;
}
