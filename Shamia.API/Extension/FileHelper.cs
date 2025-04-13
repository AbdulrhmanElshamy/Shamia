using Shamia.API.Services.interFaces;

namespace Shamia.API.Extension
{
    public  static class FileHelper
    {
        public static async Task<string> SaveImageAsync(this IFormFile image, IFileStorageService fileStorageService)
        {
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            return await fileStorageService.SaveFileAsync(imageBytes);
        }
    }
}
