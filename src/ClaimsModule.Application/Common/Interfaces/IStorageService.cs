namespace ClaimsModule.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(string blobPath, Stream content, string contentType, CancellationToken cancellationToken = default);
    Task<string> GetDownloadUrlAsync(string blobPath, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default);
}
