using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

using datingApp.api.Data;
using datingApp.api.Entities;
using datingApp.api.DTOs;
using datingApp.api.Interfaces;
using System.Linq;
using AutoMapper;

namespace datingApp.api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            this.tokenService = tokenService;
            this.mapper = mapper;
            this.context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user = this.mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            this.context.Users.Add(user);

            await this.context.SaveChangesAsync();

            return new UserDto { Username = user.UserName, token = this.tokenService.CreateToken(user), KnownAs = user.KnownAs };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await this.context.Users.Include(p => p.UserPhotos).SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDto
            {
                Username = user.UserName,
                token = this.tokenService.CreateToken(user),
                UserMainDisplayImageUrl = user.UserPhotos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await this.context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}