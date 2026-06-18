using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EcoSENA.Api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EcoSENA.Api.Services
{
    public class CloudinaryService(Cloudinary cloudinary) : ICloudinaryService
    {
        public async Task<string> UploadImageAsync(IFormFile? image, string folderName)
        {
            if (image == null || image.Length == 0)
            {
                return null;
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(image.FileName, image.OpenReadStream()),
                AssetFolder = folderName
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            string res = uploadResult.SecureUrl.ToString();
            return res;
        }
    }
}
