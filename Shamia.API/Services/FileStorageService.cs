using Shamia.API.Services.interFaces;

namespace Shamia.API.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadPath;

        public FileStorageService(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;

            string webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            _uploadPath = Path.Combine(webRoot, "Images");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> SaveFileAsync(byte[] fileBytes)
        {
            string fileName = GenerateFileName(fileBytes);
            bool isSaved = await SaveFileToStorage(fileBytes, fileName);
            if (!isSaved)
                throw new Exception("Error: Could not save the file!");

            return fileName;
        }

        private async Task<bool> SaveFileToStorage(byte[] fileBytes, string fileName)
        {
            string fileAbsolutePath = Path.Combine(_uploadPath, fileName);

            try
            {
                await using var fileStream = new FileStream(fileAbsolutePath, FileMode.Create);
                await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
                return false;
            }
        }

        private static string GenerateFileName(byte[] fileBytes)
        {
            string fileExtension = FileExtesnionHelper.TryGetExtension(fileBytes);
            string randomString = Guid.NewGuid().ToString().Replace("-", "");
            return $"{randomString}.{fileExtension}";
        }

        public string CombinePathAndFile(string fileName)
        {
            return Path.Combine(_uploadPath, fileName);
        }
    }
}
