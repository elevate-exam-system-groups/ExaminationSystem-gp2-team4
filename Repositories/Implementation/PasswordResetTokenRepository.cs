using Examination_System.Common.Models.Identity;
using Examination_System.Common.Repositories;
using ExaminationSystem.API.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository(AppDbContext context)
        : IPasswordResetTokenRepository
    {
        public async Task AddAsync(PasswordResetToken token, CancellationToken ct)
        {
            await context.PasswordResetTokens.AddAsync(token, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task<PasswordResetToken?> GetActiveTokenByUserIdAsync(
           Guid userId,
           CancellationToken ct)
        {
            return await context.PasswordResetTokens
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow,
                    ct);
        }
        public async Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct)
        {
            return await context.PasswordResetTokens
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        }

        public async Task UpdateAsync(PasswordResetToken token, CancellationToken ct)
        {
            context.PasswordResetTokens.Update(token);
            await context.SaveChangesAsync(ct);
        }
    }
}