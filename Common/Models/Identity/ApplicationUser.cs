using Microsoft.AspNetCore.Identity;

namespace Examination_System.Common.Models.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string FullName { get; set; } = string.Empty;
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiresAt { get; set; }
        public string ResetToken { get; set; } = string.Empty;
        public DateTime? ResetTokenExpiresAt { get; set; }
    }
}
