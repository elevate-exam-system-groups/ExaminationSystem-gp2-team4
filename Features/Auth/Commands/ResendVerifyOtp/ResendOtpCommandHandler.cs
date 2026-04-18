using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using Examination_System.Common.Service.Auth;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IOtpService otpService,
        IEmailService emailService)
        : IRequestHandler<ResendOtpCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. Normalize input
            var email = request.email?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse<string>.Failure(ErrorCode.InvalidCredentials);
            }

            // 2. Find user
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return ApiResponse<string>.Failure(ErrorCode.UserNotFound);
            }

            // 3. Email must exist 
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return ApiResponse<string>.Failure(ErrorCode.InvalidCredentials);
            }

            // 4. Already verified
            if (user.EmailConfirmed)
            {
                return ApiResponse<string>.Failure(ErrorCode.EmailAlreadyExists);
            }

            // 5. Rate limit
            var canResend = await otpService.CanResendOtpAsync(email);

            if (!canResend)
            {
                return ApiResponse<string>.Failure(ErrorCode.TooManyOtpRequests);
            }

            // 6. Generate OTP
            var otp = await otpService.GenerateOtp();

            // 7. Save OTP
            await otpService.SaveOtpAsync(email, otp);

            // 8. Send email (SAFE NOW)
            await emailService.SendEmailAsync(
                user.Email,
                "Your OTP Code",
                $"Your OTP is: {otp}"
            );

            return ApiResponse<string>.Success("OTP resent successfully.");
        }
    }
}