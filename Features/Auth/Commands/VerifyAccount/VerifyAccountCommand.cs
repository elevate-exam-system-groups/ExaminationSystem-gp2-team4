using Examination_System.Common.Wrappers;
using MediatR;

namespace Examination_System.Features.Auth.Commands.VerifyAccount
{
    public record VerifyAccountCommand(string Email, string Otp)
        : IRequest<ApiResponse<string>>;
}