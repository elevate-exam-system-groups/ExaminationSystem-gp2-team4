using Examination_System.Common.Models.Identity;
using ExaminationSystem.API.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Common.Service.Auth
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _dbContext;

        public OtpService(AppDbContext dbContext)
        {
            _dbContext  = dbContext;
        }
        public async Task<String> GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task SaveOtpAsync(string email, string otp)
        {
            var otpEntity = new OtpCode
            {
                Email = email,
                Code = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };
            _dbContext.OtpCodes.Add(otpEntity);
           await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var record = await _dbContext.OtpCodes
                .FirstOrDefaultAsync(x =>
                    x.Email == email &&
                    x.Code == otp &&
                    !x.IsUsed &&
                    x.ExpiryTime > DateTime.UtcNow);

            if (record == null)
                return false;

            record.IsUsed = true;
            await _dbContext.SaveChangesAsync();

            return true;
        }

      
    }
}
