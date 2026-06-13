using Microsoft.AspNetCore.Http;
namespace CarAutomotive.Application.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName); // Returns the URL of the uploaded file
        Task DeleteFileAsync(string filePath); // Deletes the file at the specified path
    }
}
