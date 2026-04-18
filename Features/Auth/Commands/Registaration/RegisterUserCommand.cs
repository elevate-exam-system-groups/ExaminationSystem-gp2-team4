using Examination_System.Common.Wrappers;
using MediatR;

namespace Examination_System.Features.Auth.Commands.Registeration
{
    public record RegisterUserCommand(string FullName, string Email, string Password)
        : IRequest<ApiResponse<RegisterUserResponse>>;

    public record RegisterUserResponse(Guid UserId, string Message);
}