using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WebAPI_WebApp.BlobStorage;

public sealed class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["BlobStorage:ConnectionString"];
        var containerName = configuration["BlobStorage:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("BlobStorage:ConnectionString is required.");

        if (string.IsNullOrWhiteSpace(containerName))
            throw new InvalidOperationException("BlobStorage:ContainerName is required.");

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var safeFileName = Path.GetFileName(file.FileName);
        var blobName = $"{Guid.NewGuid():N}-{safeFileName}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
        // Change tier to Archive
        await blobClient.SetAccessTierAsync(AccessTier.Archive);
        return blobClient.Uri.ToString();
    }
}