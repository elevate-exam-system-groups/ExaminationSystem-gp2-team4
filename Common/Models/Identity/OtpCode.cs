namespace Examination_System.Common.Models.Identity
{
    public class OtpCode
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }

        public string? HashedCode { get; set; }

        public DateTime ExpiryTime { get; set; }

        public int AttemptsCount { get; set; } = 0;

        public bool IsLocked { get; set; } = false;

        public int ResendCount { get; set; } = 0;

        public DateTime? LastResendTime { get; set; }
    }
}
