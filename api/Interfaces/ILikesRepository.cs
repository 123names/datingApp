using System.Threading.Tasks;
using api.DTOs;
using api.Entities;
using api.Helpers;
using datingApp.api.Entities;

namespace api.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}