using Examination_System.Common.Models.Identity;
using Examination_System.Common.Service.Auth;
using Examination_System.Common.Service.Enums;
using ExaminationSystem.API.Common.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public class OtpService : IOtpService
{
    private readonly AppDbContext _db;

    public OtpService(AppDbContext db)
    {
        _db = db;
    }

    // 🔐 Secure OTP
    public Task<string> GenerateOtp()
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);

        var number = BitConverter.ToUInt32(bytes, 0) % 900000 + 100000;
        return Task.FromResult(number.ToString());
    }

    //  Hashing 
    private string HashOtp(string otp)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(otp);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public async Task SaveOtpAsync(string email, string otp)
    {
        var old = await _db.OtpCodes.FirstOrDefaultAsync(x => x.Email == email);

        if (old != null)
        {
            _db.OtpCodes.Remove(old);
        }

        var entity = new OtpCode
        {
            Email = email,
            HashedCode = HashOtp(otp),
            ExpiryTime = DateTime.UtcNow.AddMinutes(10),
            AttemptsCount = 0,
            IsLocked = false,
            ResendCount = 0,
            LastResendTime = DateTime.UtcNow
        };

        _db.OtpCodes.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<OtpVerifyResult> VerifyOtpAsync(string email, string otp)
    {
        var record = await _db.OtpCodes.FirstOrDefaultAsync(x => x.Email == email);

        if (record == null)
            return OtpVerifyResult.Invalid;

        if (record.IsLocked)
            return OtpVerifyResult.Locked;

        if (record.ExpiryTime <= DateTime.UtcNow)
            return OtpVerifyResult.Expired;

        var hashedInput = HashOtp(otp);

        if (record.HashedCode != hashedInput)
        {
            record.AttemptsCount++;

            if (record.AttemptsCount >= 5)
            {
                record.IsLocked = true;
            }

            await _db.SaveChangesAsync();
            return record.IsLocked
                ? OtpVerifyResult.Locked
                : OtpVerifyResult.Invalid;
        }

        // success
        return OtpVerifyResult.Success;
    }

    public async Task InvalidateOtpAsync(string email)
    {
        var record = await _db.OtpCodes.FirstOrDefaultAsync(x => x.Email == email);

        if (record != null)
        {
            _db.OtpCodes.Remove(record);
            await _db.SaveChangesAsync();
        }
    }

    //  resend OTP 
    public async Task<bool> CanResendOtpAsync(string email)
    {
        var record = await _db.OtpCodes.FirstOrDefaultAsync(x => x.Email == email);

        if (record == null)
            return true;

        if (record.LastResendTime.HasValue &&
            record.LastResendTime.Value.AddHours(1) < DateTime.UtcNow)
        {
            record.ResendCount = 0;
        }

        if (record.ResendCount >= 3)
            return false;

        record.ResendCount++;
        record.LastResendTime = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }
}