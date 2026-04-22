using Examination_System.Common.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Common.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastActivityAt { get; set; }

        public Student? Student { get; set; }

        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
            = new List<PasswordResetToken>();

        public ICollection<Attempt> Attempts { get; set; }
            = new List<Attempt>();
    }
}
