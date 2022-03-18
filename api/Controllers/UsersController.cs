using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using datingApp.api.Interfaces;
using datingApp.api.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using api.Extensions;
using api.Interfaces;
using datingApp.api.Entities;
using api.Helpers;

namespace datingApp.api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;
        private readonly IUnitOfWork unitOfWork;

        public UsersController(IMapper mapper, IPhotoService photoService, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        // api end-point for get other user details
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var gender = await this.unitOfWork.userRepository.GetUserGender(User.GetUserName());
            userParams.CurrentUserName = User.GetUserName();

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";

            var users = await this.unitOfWork.userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalItemCount, users.TotalPages);

            return Ok(users);
        }

        // api end-point for one user details
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var currentUsername = User.GetUserName();
            return await this.unitOfWork.userRepository.GetMemberByUsernameAsync(username, isCurrentUser: username == currentUsername);
        }

        // api end-point for edit one user profiles
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await this.unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());
            this.mapper.Map(memberUpdateDto, user);

            this.unitOfWork.userRepository.Update(user);
            if (await this.unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to update user");
        }

        // api end-point for upload a image
        [HttpPost("add-photo")]
        public async Task<ActionResult<UserPhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await this.unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());
            var result = await this.photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var newPhoto = new UserPhoto
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user.UserPhotos.Add(newPhoto);

            if (await this.unitOfWork.Complete())
            {
                // return this.mapper.Map<UserPhotoDto>(newPhoto);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, this.mapper.Map<UserPhotoDto>(newPhoto));
            }

            return BadRequest("Error adding new photo");
        }

        // api end-point for change a image as main display image
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await this.unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.UserPhotos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This photo is already your main photo");

            var currentMain = user.UserPhotos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await this.unitOfWork.Complete()) return NoContent();

            return BadRequest("Fails to set main photo");
        }

        // api end-point for delete a image
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await this.unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.UserPhotos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You can't delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await this.photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.UserPhotos.Remove(photo);

            if (await this.unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to delete the photo");
        }

    }
}