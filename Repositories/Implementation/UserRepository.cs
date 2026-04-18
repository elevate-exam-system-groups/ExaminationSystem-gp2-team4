using Examination_System.Common.Models.Identity;
using Examination_System.Common.Models;
using ExaminationSystem.API.Common.Data;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Common.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct)
                 => _userManager.FindByEmailAsync(email);

        public Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken ct)
            => _userManager.FindByNameAsync(username)!;


        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
        public async Task<string?> GetRoleAsync(ApplicationUser user, CancellationToken ct)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }
    }
}
