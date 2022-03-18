
using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using datingApp.api.Entities;

namespace api.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task<UserPhoto> GetPhotoById(int id);
        void RemovePhoto(UserPhoto photo);
    }
}