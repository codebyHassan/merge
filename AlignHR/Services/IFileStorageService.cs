using Microsoft.AspNetCore.Http;

namespace AlignHR.Services
{
    public interface IFileStorageService
    {
        Task<(string FileName, string RelativePath)> SaveAsync(IFormFile file, string subFolder);
        void Delete(string relativePath);
        string GetAbsolutePath(string relativePath);
    }
}
