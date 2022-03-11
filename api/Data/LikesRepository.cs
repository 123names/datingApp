
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.Entities;
using api.Helpers;
using api.Interfaces;
using datingApp.api.Data;
using datingApp.api.Entities;
using datingApp.api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class LikesRepository : ILikesRepository
    {
        public DataContext Context { get; }
        public LikesRepository(DataContext context)
        {
            this.Context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await this.Context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = this.Context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = this.Context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }
            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                Id = user.Id,
                Username = user.UserName,
                KnownAs = user.KnownAs,
                City = user.City,
                Age = user.DateOfBirth.CalculateAge(),
                MainPhotoUrl = user.UserPhotos.FirstOrDefault(p => p.IsMain).Url,

            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await this.Context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}