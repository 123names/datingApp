using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.Interfaces;
using datingApp.api.Data;
using datingApp.api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext context;

        public PhotoRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserPhoto> GetPhotoById(int id)
        {
            return await this.context.UserPhotos.IgnoreQueryFilters().SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await this.context.UserPhotos
                .IgnoreQueryFilters()
                .Where(p => p.IsApproved == false)
                .Select(up => new PhotoForApprovalDto
                {
                    Id = up.Id,
                    Username = up.AppUser.UserName,
                    Url = up.Url,
                    IsApproved = up.IsApproved
                }).ToListAsync();
        }

        public void RemovePhoto(UserPhoto photo)
        {
            this.context.UserPhotos.Remove(photo);
        }
    }
}