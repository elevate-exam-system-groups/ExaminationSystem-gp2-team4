using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Examination_System.Common.Service.Auth
{
    public interface IOtpService
    {
        Task<string> GenerateOtp();
        Task SaveOtpAsync(string email,String otp);

        Task<bool> VerifyOtpAsync(string email,String otp);
    }
}
