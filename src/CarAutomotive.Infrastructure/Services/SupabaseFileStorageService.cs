using CarAutomotive.Application.Common.Settings;
using CarAutomotive.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Supabase;

namespace CarAutomotive.Infrastructure.Services
{
    public class SupabaseFileStorageService : IFileStorageService
    {
        private readonly SupabaseSettings _settings;

        public SupabaseFileStorageService(
            IOptions<SupabaseSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<string> UploadFileAsync(
            IFormFile file,
            string folderName)
        {
            var client = new Client(
                _settings.Url,
                _settings.Key);

            await client.InitializeAsync();

            var extension = Path.GetExtension(file.FileName);

            var fileName =
                $"{Guid.NewGuid()}{extension}";

            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            var bytes = memoryStream.ToArray();

            await client.Storage
                .From("products")
                .Upload(
                    bytes,
                    fileName);

            var publicUrl = client.Storage
                .From("products")
                .GetPublicUrl(fileName);

            return publicUrl;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var client = new Client(
                _settings.Url,
                _settings.Key);

            await client.InitializeAsync();

            var fileName = Path.GetFileName(
                new Uri(filePath).AbsolutePath);

            await client.Storage
                .From("products")
                .Remove(
                    new List<string>
                    {
                        fileName
                    });
        }
    }
}