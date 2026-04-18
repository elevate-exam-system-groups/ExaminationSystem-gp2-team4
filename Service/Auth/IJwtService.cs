using Examination_System.Common.Models;

namespace Examination_System.Common.Service.Auth
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);

        string GenerateRefreshToken(ApplicationUser user);
    }
}
