using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IPasswordResetTokenRepository tokenRepo)
        : IRequestHandler<ResetPasswordCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            // 1. Find user
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return ApiResponse<string>.Failure(
                    ErrorCode.InvalidOperation
                    
                );

            // 2. Find token (better: deterministic lookup)
            var token = await tokenRepo.GetActiveTokensByUserIdAsync(user.Id, ct);

            if (token == null)
                return ApiResponse<string>.Failure(
                    ErrorCode.InvalidOperation
                    
                );

            // 3. Validate token ownership + expiry + usage
            if (token.UserId != user.Id ||
                token.IsUsed ||
                token.ExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<string>.Failure(
                    ErrorCode.InvalidOperation
                    
                );
            }

            // 4. Verify token securely
            var isValid = BCrypt.Net.BCrypt.Verify(request.Token, token.TokenHash);

            if (!isValid)
                return ApiResponse<string>.Failure(
                    ErrorCode.InvalidOperation
                   
                );

            // 5. Reset password 
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.NewPassword);

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return ApiResponse<string>.Failure(ErrorCode.OperationFailed);

            // 6. Invalidate token (single use)
            token.IsUsed = true;
            await tokenRepo.UpdateAsync(token, ct);

            return ApiResponse<string>.Success(
                "Password has been reset successfully."
            );
        }
    }
}