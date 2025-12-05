using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace PicHub.AlbumUploader.Services.Storage;

public class AzureBlobService : IBlobService
{
    private readonly BlobServiceClient _client;

    public AzureBlobService(BlobServiceClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task CreateContainerIfNotExistsAsync(string containerName, CancellationToken cancellationToken = default)
    {
        var container = _client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        // Only enable public blob access and relaxed CORS in non-production/dev environments.
        // Controlled via environment variable `ENABLE_DEV_BLOB_ACCESS` (set to "1" for dev).
        var enableDev = string.Equals(Environment.GetEnvironmentVariable("ENABLE_DEV_BLOB_ACCESS"), "1", StringComparison.OrdinalIgnoreCase);
        if (!enableDev) return;

        try
        {
            await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob, cancellationToken: cancellationToken);
        }
        catch
        {
            // Ignore failures to set access policy (e.g., insufficient permissions in production)
        }

        try
        {
            var props = await _client.GetPropertiesAsync(cancellationToken: cancellationToken);
            var serviceProps = props.Value;
            serviceProps.Cors = new System.Collections.Generic.List<Azure.Storage.Blobs.Models.BlobCorsRule>
            {
                new Azure.Storage.Blobs.Models.BlobCorsRule
                {
                    AllowedHeaders = "*",
                    AllowedMethods = "GET,HEAD",
                    AllowedOrigins = "*",
                    ExposedHeaders = "*",
                    MaxAgeInSeconds = 3600
                }
            };

            await _client.SetPropertiesAsync(serviceProps, cancellationToken: cancellationToken);
        }
        catch
        {
            // Ignore CORS config errors in environments where not permitted
        }
    }

    public async Task UploadAsync(string containerName, string blobName, Stream data, CancellationToken cancellationToken = default)
    {
        var container = _client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(data, overwrite: true, cancellationToken: cancellationToken);
    }

    public async Task DeleteIfExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var container = _client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public Task<Uri> GetBlobUriAsync(string containerName, string blobName)
    {
        var container = _client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        return Task.FromResult(blob.Uri);
    }

    public async Task<Stream> OpenReadAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var container = _client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        var resp = await blob.DownloadAsync(cancellationToken: cancellationToken);
        return resp.Value.Content;
    }
}
