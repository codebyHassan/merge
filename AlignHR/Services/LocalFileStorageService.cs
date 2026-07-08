using Microsoft.AspNetCore.Http;

namespace AlignHR.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        // Security: only these types may be stored. Deliberately excludes executable/
        // active-content types (.html, .htm, .svg, .js, .exe, .aspx, …) so an uploaded
        // file can never be served back as same-origin script (stored XSS).
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png"
        };

        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config)
        {
            var configPath = config["FileStorage:BasePath"] ?? "uploads/candidates";
            _basePath = Path.Combine(env.WebRootPath, configPath);
        }

        public async Task<(string FileName, string RelativePath)> SaveAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file was provided.");

            if (file.Length > MaxFileSizeBytes)
                throw new InvalidOperationException($"File exceeds the {MaxFileSizeBytes / (1024 * 1024)} MB size limit.");

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? string.Empty;
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Unsupported file type. Allowed: PDF, DOC, DOCX, JPG, PNG.");

            var folder = Path.Combine(_basePath, subFolder);
            Directory.CreateDirectory(folder);

            // Filename is a server-generated GUID + validated extension only — the
            // client-supplied name never touches the filesystem path.
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

            if (!TryResolveWithinRoot(relativePath, out var absolute)) return;

            if (File.Exists(absolute))
                File.Delete(absolute);
        }

        public string GetAbsolutePath(string relativePath)
        {
            if (!TryResolveWithinRoot(relativePath, out var absolute))
                throw new InvalidOperationException("Invalid file path.");

            return absolute;
        }

        /// <summary>
        /// Resolves a stored relative path to an absolute path and verifies it stays
        /// within the storage root, blocking path-traversal (e.g. "../../appsettings.json").
        /// </summary>
        private bool TryResolveWithinRoot(string relativePath, out string absolute)
        {
            absolute = string.Empty;
            if (string.IsNullOrWhiteSpace(relativePath)) return false;

            var root = Path.GetFullPath(Path.GetDirectoryName(_basePath)!);

            var parts = relativePath.TrimStart('/', '\\').Split('/', '\\');
            var candidate = Path.GetFullPath(Path.Combine(root, Path.Combine(parts)));

            var rootWithSep = root.EndsWith(Path.DirectorySeparatorChar)
                ? root
                : root + Path.DirectorySeparatorChar;

            if (!candidate.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase))
                return false;

            absolute = candidate;
            return true;
        }
    }
}
