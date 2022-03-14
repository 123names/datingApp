using System.Collections.Generic;
using System.Threading.Tasks;
using api.Helpers;
using datingApp.api.DTOs;
using datingApp.api.Entities;

namespace datingApp.api.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberByUsernameAsync(string username);
        Task<string> GetUserGender(string username);
    }
}