namespace CarAutomotive.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
