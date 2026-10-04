using CarAutomotive.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
namespace CarAutomotive.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IHostEnvironment _environment;

        public LocalFileStorageService(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var uploadsFolder = Path.Combine(
                Path.Combine(_environment.ContentRootPath, "wwwroot"),
                "uploads",
                folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileExtension = Path.GetExtension(file.FileName);

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(fileStream);

            return $"/uploads/{folderName}/{uniqueFileName}";
        }

        public Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.CompletedTask;

            var relativePath = filePath.TrimStart('/');

            var fullPath = Path.Combine(Path.Combine(_environment.ContentRootPath, "wwwroot"), relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }
    }
}