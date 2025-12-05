using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PicHub.AlbumUploader.Services.Storage;

public interface IBlobService
{
    Task CreateContainerIfNotExistsAsync(string containerName, CancellationToken cancellationToken = default);
    Task UploadAsync(string containerName, string blobName, Stream data, CancellationToken cancellationToken = default);
    Task DeleteIfExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    Task<Uri> GetBlobUriAsync(string containerName, string blobName);
}
