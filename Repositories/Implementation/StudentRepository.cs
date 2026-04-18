using Examination_System.Common.Models.Identity;
using ExaminationSystem.API.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Common.Repositories.Implementaition
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _db;

        public StudentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Student?> GetByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _db.Students
                .FirstOrDefaultAsync(x => x.Id == userId, ct);
        }

        public async Task<Guid?> GetIdByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _db.Students
                .Where(x => x.Id == userId)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(Student student, CancellationToken ct)
        {
            _db.Students.Add(student);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Student student, CancellationToken ct)
        {
            _db.Students.Update(student);
            await _db.SaveChangesAsync(ct);
        }
    }
}