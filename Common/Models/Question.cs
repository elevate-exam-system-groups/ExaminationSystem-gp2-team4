using System;
using Examination_System.Common.Models;

namespace Examination_System.Common.Models
{
    public class Question : BaseEntity
    {
        public Guid QuizId { get; set; }
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

<<<<<<< HEAD
        public Quiz? Quiz { get; set; }
=======
        public Quiz Quiz { get; set; }
>>>>>>> origin/Test
        public ICollection<Option> Options { get; set; } = new List<Option>();
    }
}
