namespace Examination_System.Common.Models.Identity
{
    public class OtpCode
    {
        public Guid Id { get; set; }

        public string? Email { get; set; }
        public string? Code { get; set; }

        public DateTime ExpiryTime { get; set; }

        public bool IsUsed { get; set; }
    }
}
