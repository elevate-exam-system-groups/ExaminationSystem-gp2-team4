using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using Examination_System.Common.Repositories;
using Examination_System.Common.Service.Auth;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.Auth.Commands.Login
{
    public class LoginUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IStudentRepository studentRepo)
        : IRequestHandler<LoginUserCommand, ApiResponse<LoginResponse>>
    {
        public async Task<ApiResponse<LoginResponse>> Handle(LoginUserCommand request, CancellationToken ct)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(request.email) ||
                string.IsNullOrWhiteSpace(request.password))
            {
                return ApiResponse<LoginResponse>.Failure(ErrorCode.InvalidCredentials);
            }

            // 2. Normalize email 
            var normalizedEmail = request.email?.Trim().ToUpperInvariant();

            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, ct);

            if (user == null)
            {
                return ApiResponse<LoginResponse>.Failure(ErrorCode.InvalidCredentials);
            }

            // 4. Email confirmation check
            if (!user.EmailConfirmed)
            {
                return ApiResponse<LoginResponse>.Failure(ErrorCode.EmailNotConfirmed);
            }

            // 5. Lockout check
            if (await userManager.IsLockedOutAsync(user))
            {
                return ApiResponse<LoginResponse>.Failure(ErrorCode.AccountLocked);
            }

            // 6. Password check
            var passwordValid = await userManager.CheckPasswordAsync(user, request.password);

            if (!passwordValid)
            {
                await userManager.AccessFailedAsync(user);

                if (await userManager.IsLockedOutAsync(user))
                {
                    return ApiResponse<LoginResponse>.Failure(ErrorCode.TooManyLoginAttempts);
                }

                return ApiResponse<LoginResponse>.Failure(ErrorCode.InvalidCredentials);
            }

            // 7. Reset failed count
            await userManager.ResetAccessFailedCountAsync(user);

            // 8. Roles
            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Unknown";
            var isStudent = roles.Contains("Student");

            // 9. Student checks
            if (isStudent)
            {
                var student = await studentRepo.GetByUserIdAsync(user.Id, ct);

                if (student == null)
                {
                    return ApiResponse<LoginResponse>.Failure(ErrorCode.UserNotFound);
                }

                if (student.Status == AccountStatus.Locked)
                    return ApiResponse<LoginResponse>.Failure(ErrorCode.AccountLocked);

                if (student.Status is AccountStatus.Pending or AccountStatus.Suspended)
                    return ApiResponse<LoginResponse>.Failure(ErrorCode.Forbidden);
            }

            // 10. Generate tokens
            var accessToken = jwtService.GenerateToken(user, roles);
            var refreshToken = jwtService.GenerateRefreshToken(user);
            
            user.LastLoginAt = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            // 11. Response
            return ApiResponse<LoginResponse>.Success(
                new LoginResponse(
                    Token: accessToken,
                    RefreshToken: refreshToken,
                    Role: role,
                    UserId: user.Id,
                    FullName: user.FullName
                )
            );
        }
    }
}