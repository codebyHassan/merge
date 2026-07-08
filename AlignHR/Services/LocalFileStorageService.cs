using Microsoft.AspNetCore.Http;

namespace AlignHR.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config)
        {
            var configPath = config["FileStorage:BasePath"] ?? "uploads/candidates";
            _basePath = Path.Combine(env.WebRootPath, configPath);
        }

        public async Task<(string FileName, string RelativePath)> SaveAsync(IFormFile file, string subFolder)
        {
            var folder = Path.Combine(_basePath, subFolder);
            Directory.CreateDirectory(folder);

            var extension = Path.GetExtension(file.FileName);
            var uniqueName = $"{Guid.NewGuid():N}{extension}";
            var absolutePath = Path.Combine(folder, uniqueName);

            using var stream = new FileStream(absolutePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = Path.Combine("uploads", "candidates", subFolder, uniqueName)
                .Replace('\\', '/');

            return (file.FileName, relativePath);
        }

        public void Delete(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var parts = relativePath.TrimStart('/').Split('/');
            var absolute = Path.Combine(
                Path.GetDirectoryName(_basePath)!,
                Path.Combine(parts));

            if (File.Exists(absolute))
                File.Delete(absolute);
        }

        public string GetAbsolutePath(string relativePath)
        {
            var parts = relativePath.TrimStart('/').Split('/');
            return Path.Combine(
                Path.GetDirectoryName(_basePath)!,
                Path.Combine(parts));
        }
    }
}
