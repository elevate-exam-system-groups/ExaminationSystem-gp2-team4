using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using Examination_System.Common.Repositories;
using Examination_System.Common.Service.Auth;
using Examination_System.Common.Service.Enums;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Features.Auth.Commands.VerifyAccount
{
    public class VerifyAccountCommandHandler(
        UserManager<ApplicationUser> userManager,
        IOtpService otpService,
        IStudentRepository studentRepo)
        : IRequestHandler<VerifyAccountCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(VerifyAccountCommand request, CancellationToken ct)
        {
            //  Validation
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Otp))
            {
                return ApiResponse<string>.Failure(ErrorCode.InvalidOtp);
            }

            //  Get user
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return ApiResponse<string>.Failure(ErrorCode.UserNotFound);
            }

            //  Already verified
            if (user.EmailConfirmed)
            {
                return ApiResponse<string>.Failure(ErrorCode.EmailAlreadyExists);
            }

            //  Verify OTP
            var otpResult = await otpService.VerifyOtpAsync(request.Email, request.Otp);

            switch (otpResult)
            {
                case OtpVerifyResult.Expired:
                    return ApiResponse<string>.Failure(ErrorCode.OtpExpired);

                case OtpVerifyResult.Locked:
                    return ApiResponse<string>.Failure(ErrorCode.TooManyOtpRequests);

                case OtpVerifyResult.Invalid:
                    return ApiResponse<string>.Failure(ErrorCode.InvalidOtp);
            }

            //  Confirm email
            user.EmailConfirmed = true;

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return ApiResponse<string>.Failure(ErrorCode.InternalServerError);
            }

            //  Activate student
            var student = await studentRepo.GetByUserIdAsync(user.Id, ct);

            if (student == null)
            {
                return ApiResponse<string>.Failure(ErrorCode.UserNotFound);
            }

            student.Status = AccountStatus.Active;
            await studentRepo.UpdateAsync(student, ct);

            //  Invalidate OTP
            await otpService.InvalidateOtpAsync(request.Email);

            return ApiResponse<string>.Success("Account verified successfully.");
        }
    }
}