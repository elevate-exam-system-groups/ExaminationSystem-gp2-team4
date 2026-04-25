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
            //  Find user
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return ApiResponse<string>.Failure(
                    ErrorCode.InvalidOperation
                    
                );

            //  Find token 
            var token = await tokenRepo.GetActiveTokenByUserIdAsync(user.Id, ct);

            if (token == null)
                return ApiResponse<string>.Failure(
                    ErrorCode.InvalidOperation
                    
                );

            // Validate token ownership + expiry + usage
            if (token.UserId != user.Id ||
                token.IsUsed ||
                token.ExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<string>.Failure(
                    ErrorCode.InvalidOperation
                    
                );
            }

            //  Verify token securely
            var isValid = BCrypt.Net.BCrypt.Verify(request.Token, token.TokenHash);

            if (!isValid)
                return ApiResponse<string>.Failure(
                    ErrorCode.InvalidOperation
                   
                );

            //  Reset password 
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.NewPassword);

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return ApiResponse<string>.Failure(ErrorCode.OperationFailed);

            //  Invalidate token 
            token.IsUsed = true;
            await tokenRepo.UpdateAsync(token, ct);

            return ApiResponse<string>.Success(
                "Password has been reset successfully."
            );
        }
    }
}