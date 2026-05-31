using ClaimsModule.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ClaimsModule.Infrastructure.Storage;

public class LocalFileSystemStorageService(IConfiguration configuration) : IStorageService
{
    private string BasePath => configuration["Storage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    public async Task<string> UploadAsync(string blobPath, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(BasePath, blobPath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var file = File.Create(fullPath);
        await content.CopyToAsync(file, cancellationToken);
        return fullPath;
    }

    public Task<string> GetDownloadUrlAsync(string blobPath, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(BasePath, blobPath.Replace('/', Path.DirectorySeparatorChar));
        return Task.FromResult($"file:///{fullPath.Replace('\\', '/')}");
    }

    public Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(BasePath, blobPath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
