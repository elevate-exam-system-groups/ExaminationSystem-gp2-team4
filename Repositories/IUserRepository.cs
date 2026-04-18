using Examination_System.Common.Models.Identity;
using Examination_System.Common.Models;

namespace Examination_System.Common.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<string?> GetRoleAsync(ApplicationUser user, CancellationToken ct = default);
    }
}
