using System.Security.Claims;

namespace CarAutomotive.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);


    }
}
