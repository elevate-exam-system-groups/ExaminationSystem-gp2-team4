using Examination_System.Common.Wrappers;
using MediatR;

namespace Examination_System.Features.Auth.Commands.Login
{
    public record LoginUserCommand(string email, string password)
        : IRequest<ApiResponse<LoginResponse>>;

    public record LoginResponse(
        string Token,
        string RefreshToken,
        string Role,
        Guid UserId,
        string FullName);
}