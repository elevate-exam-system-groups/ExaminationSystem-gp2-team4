using System;
using System.Collections.Generic;

namespace Examination_System.Features.Attempts.DTOs
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<OptionDto> Options { get; set; } = new();
        public Guid QuizId { get; set; }
    }
}
