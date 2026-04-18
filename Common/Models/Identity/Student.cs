namespace Examination_System.Common.Models.Identity
{

    public class Student
    {
        public Guid Id { get; set; }

        public AccountStatus Status { get; set; }

        public ApplicationUser User { get; set; }
    }
    public enum AccountStatus
    {
        Pending,
        Active,
        Locked,
        Suspended
    }

}
