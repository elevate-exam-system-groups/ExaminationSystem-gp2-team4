using Examination_System.Common.Models.Identity;

namespace Examination_System.Common.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByUserIdAsync(Guid userId, CancellationToken ct);

        Task AddAsync(Student student, CancellationToken ct);

        Task UpdateAsync(Student student, CancellationToken ct);
    }
}