namespace Examination_System.Common.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
<<<<<<< HEAD
        public DateTime? UpdatedAt { get; set; }
=======
        public DateTime UpdatedAt { get; set; }
>>>>>>> origin/Test
    }
}
