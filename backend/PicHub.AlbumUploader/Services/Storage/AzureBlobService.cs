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
}
