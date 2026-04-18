using Examination_System.Common.Wrappers;
using MediatR;

namespace Examination_System.Features.Auth.Commands.ResendOtp
{
    public record ResendOtpCommand(string email)
        : IRequest<ApiResponse<string>>;
}