using Examination_System.Common.Models.Identity;
using Examination_System.Common.Models;

namespace Examination_System.Common.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        Task AddAsync(PasswordResetToken token, CancellationToken ct);

        Task<PasswordResetToken?> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken ct);

        Task UpdateAsync(PasswordResetToken token, CancellationToken ct);
        //Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct);

    }

}