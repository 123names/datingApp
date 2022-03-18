using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace api.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var cloudinaryAccount = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            this.cloudinary = new Cloudinary(cloudinaryAccount);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadImageParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("scale")
                };
                uploadResult = await this.cloudinary.UploadAsync(uploadImageParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteImageParams = new DeletionParams(publicId);
            var deletionResult = await this.cloudinary.DestroyAsync(deleteImageParams);

            return deletionResult;
        }
    }
}