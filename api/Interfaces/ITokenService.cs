using datingApp.api.Entities;

namespace datingApp.api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}