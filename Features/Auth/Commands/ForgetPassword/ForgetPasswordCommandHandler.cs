using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using Examination_System.Common.Repositories;
using Examination_System.Common.Service.Auth;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Features.Auth.Commands.ForgetPassword
{
    public class ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IPasswordResetTokenRepository tokenRepo)
        : IRequestHandler<ForgotPasswordCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            //  Validate input
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return ApiResponse<string>.Failure(ErrorCode.ValidationError);
            }

            //  Find user
            var user = await userManager.FindByEmailAsync(request.Email);

              if (user == null)
            {
                return ApiResponse<string>.Success(
                    " the email Not exists,."
                );
            }

            //   invalidate old tokens
            var oldToken = await tokenRepo.GetActiveTokenByUserIdAsync(user.Id, ct);

            if (oldToken != null)
            {
                oldToken.IsUsed = true;
                await tokenRepo.UpdateAsync(oldToken, ct);
            }

            //  Generate secure raw token
            var rawToken = Guid.NewGuid().ToString("N");

            //  Hash token before storing
            var tokenHash = BCrypt.Net.BCrypt.HashPassword(rawToken);

            //  Store token
            await tokenRepo.AddAsync(new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            }, ct);

            //  Build frontend reset link
            var resetLink =
                $"https://localhost:7124/reset-password?token={rawToken}&email={user.Email}";

            // Send email
            await emailService.SendEmailAsync(
                user.Email,
                "Reset Password Request",
                $@"
                    <h3>Reset Your Password</h3>
                    <p>You requested to reset your password.</p>
                    <p>
                        <a href='{resetLink}'>Click here to reset your password</a>
                    </p>
                    <p>This link will expire in 15 minutes.</p>
                    <p>If you did not request this, ignore this email.</p>
                "
            );

            return ApiResponse<string>.Success(
                "Please Check Your Email And Enter To The Links."
            );
        }
    }
}