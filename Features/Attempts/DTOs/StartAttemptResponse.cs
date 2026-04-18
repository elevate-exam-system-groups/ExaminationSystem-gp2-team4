using System;
using System.Collections.Generic;

namespace Examination_System.Features.Attempts.DTOs
{
    public class StartAttemptResponse
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }
}
