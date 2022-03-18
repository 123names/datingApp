namespace api.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPhotoService photoService;

        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.photoService = photoService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRole()
        {
            var users = await this.userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();
            var user = await this.userManager.FindByNameAsync(username);
            if (user == null) return NotFound("Could not find user");
            var userRoles = await this.userManager.GetRolesAsync(user);

            var result = await this.userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add roles to user");

            result = await this.userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove  roles from user");

            return Ok(await this.userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "RequireAtLeastModerateRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            var photos = await this.unitOfWork.photoRepository.GetUnapprovedPhotos();
            return Ok(photos);
        }

        [Authorize(Policy = "RequireAtLeastModerateRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await this.unitOfWork.photoRepository.GetPhotoById(photoId);
            if (photo == null) return NotFound("Could not find photo");
            photo.IsApproved = true;

            var user = await this.unitOfWork.userRepository.GetUserByPhotoId(photoId);
            if (!user.UserPhotos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }
            await this.unitOfWork.Complete();

            return Ok();
        }

        [Authorize(Policy = "RequireAtLeastModerateRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await this.unitOfWork.photoRepository.GetPhotoById(photoId);
            if (photo.PublicId != null)
            {
                var result = await this.photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Result == "ok")
                {
                    this.unitOfWork.photoRepository.RemovePhoto(photo);
                }
            }
            else
            {
                this.unitOfWork.photoRepository.RemovePhoto(photo);
            }
            await this.unitOfWork.Complete();

            return Ok();
        }
    }
}