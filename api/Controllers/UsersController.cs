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

namespace datingApp.api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            this.mapper = mapper;
            this.photoService = photoService;
            this.userRepository = userRepository;
        }

        // api end-point for get other user details
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await this.userRepository.GetMembersAsync();
            return Ok(users);
        }

        // api end-point for one user details
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await this.userRepository.GetMemberByUsernameAsync(username);
        }

        // api end-point for edit one user profiles
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await this.userRepository.GetUserByUsernameAsync(User.GetUserName());
            this.mapper.Map(memberUpdateDto, user);

            this.userRepository.Update(user);
            if (await this.userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }

        // api end-point for upload a image
        [HttpPost("add-photo")]
        public async Task<ActionResult<UserPhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await this.userRepository.GetUserByUsernameAsync(User.GetUserName());
            var result = await this.photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var newPhoto = new UserPhoto
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.UserPhotos.Count == 0)
            {
                newPhoto.IsMain = true;
            }

            user.UserPhotos.Add(newPhoto);

            if (await this.userRepository.SaveAllAsync())
            {
                // return this.mapper.Map<UserPhotoDto>(newPhoto);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, this.mapper.Map<UserPhotoDto>(newPhoto));
            }

            return BadRequest("Error adding new photo");
        }
    }
}