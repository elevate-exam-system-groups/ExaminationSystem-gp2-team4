using System;

namespace Examination_System.Features.Attempts.DTOs
{
    public class SubmitAttemptResponse
    {
        public Guid AttemptId { get; set; }
        public int Score { get; set; }   // 0–100
        public bool Passed { get; set; }
    }
}
