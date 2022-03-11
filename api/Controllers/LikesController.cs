
using System.Threading.Tasks;
using api.DTOs;
using api.Entities;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using datingApp.api.Controllers;
using datingApp.api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly ILikesRepository likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            this.userRepository = userRepository;
            this.likesRepository = likesRepository;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await this.userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await this.likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You can't like yourself");
            var alreadyLiked = await this.likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (alreadyLiked != null) return BadRequest("You already liked this user");

            var userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if (await this.userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Fail to like this user");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await this.likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalItemCount, users.TotalPages);
            return Ok(users);
        }
    }
}