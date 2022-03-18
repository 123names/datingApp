namespace api.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task<UserPhoto> GetPhotoById(int id);
        void RemovePhoto(UserPhoto photo);
    }
}