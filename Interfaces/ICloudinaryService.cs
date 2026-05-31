namespace EcoSENA.Api.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile image, string folderName);
    }
}
