using Examination_System.Common.Service.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Examination_System.Common.Service.Auth
{
    public interface IOtpService
    {
        Task<string> GenerateOtp();
        Task SaveOtpAsync(string email, string otp);
        Task<OtpVerifyResult> VerifyOtpAsync(string email, string otp);
        Task InvalidateOtpAsync(string email);
        Task<bool> CanResendOtpAsync(string email);
    }
}
