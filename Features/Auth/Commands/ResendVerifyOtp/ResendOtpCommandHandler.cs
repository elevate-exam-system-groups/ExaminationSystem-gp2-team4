using Examination_System.Common.Models;
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
            //  Normalize input
            var email = request.email?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse<string>.Failure(ErrorCode.InvalidCredentials);
            }

            //  Find user
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return ApiResponse<string>.Failure(ErrorCode.UserNotFound);
            }

            //  Email must exist 
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return ApiResponse<string>.Failure(ErrorCode.InvalidCredentials);
            }

            //  Already verified
            if (user.EmailConfirmed)
            {
                return ApiResponse<string>.Failure(ErrorCode.EmailAlreadyExists);
            }

            // Rate limit
            var canResend = await otpService.CanResendOtpAsync(email);

            if (!canResend)
            {
                return ApiResponse<string>.Failure(ErrorCode.TooManyOtpRequests);
            }

            //  Generate OTP
            var otp = await otpService.GenerateOtp();

            //  Save OTP
            await otpService.SaveOtpAsync(email, otp);

            //  Send email 
            await emailService.SendEmailAsync(
                user.Email,
                "Your OTP Code",
                $"Your OTP is: {otp}"
            );

            return ApiResponse<string>.Success("OTP resent successfully.");
        }
    }
}