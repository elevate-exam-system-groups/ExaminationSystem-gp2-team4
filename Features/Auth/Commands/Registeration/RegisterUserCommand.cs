using Examination_System.Common.Wrappers.ResultFormat;
using MediatR;

namespace Examination_System.Features.Auth.Commands.Registeration
{
    public record RegisterUserCommand(string FullName, string Email, string Password) : IRequest<Result<RegisterUserResponse>>;

    public record RegisterUserResponse(Guid UserId, string Message);
}
