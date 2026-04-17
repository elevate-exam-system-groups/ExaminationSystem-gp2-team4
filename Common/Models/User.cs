using Microsoft.AspNetCore.Identity;

namespace Examination_System.Common.Models
{
<<<<<<< HEAD
    public class User :BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
        public string VerficationCode { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public DateTime? ResetTokenExpiresAt { get; set; }

        public virtual ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
        
=======
    public class ApplicationUser : IdentityUser
    {
>>>>>>> Create-Manage-Quizzes
    }
}
