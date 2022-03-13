using System.Threading.Tasks;
using datingApp.api.Entities;

namespace datingApp.api.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}