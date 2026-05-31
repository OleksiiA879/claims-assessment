using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using ClaimsModule.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ClaimsModule.Infrastructure.Storage;

public class AzureBlobStorageService(IConfiguration configuration) : IStorageService
{
    private readonly string _container = configuration["Storage:Azure:Container"] ?? "claim-documents";
    private readonly BlobServiceClient _client = new(configuration["Storage:Azure:ConnectionString"]
        ?? throw new InvalidOperationException("Azure Blob connection string is required."));

    public async Task<string> UploadAsync(string blobPath, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var container = _client.GetBlobContainerClient(_container);
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blob = container.GetBlobClient(blobPath);
        await blob.UploadAsync(content, overwrite: true, cancellationToken);
        return blobPath;
    }

    public async Task<string> GetDownloadUrlAsync(string blobPath, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var container = _client.GetBlobContainerClient(_container);
        var blob = container.GetBlobClient(blobPath);
        if (!blob.CanGenerateSasUri) return blob.Uri.ToString();
        var sas = new BlobSasBuilder(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(ttl))
        {
            BlobContainerName = _container,
            BlobName = blobPath
        };
        return blob.GenerateSasUri(sas).ToString();
    }

    public async Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var container = _client.GetBlobContainerClient(_container);
        await container.DeleteBlobIfExistsAsync(blobPath, cancellationToken: cancellationToken);
    }
}
