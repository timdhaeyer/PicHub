using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PicHub.AlbumUploader.Services.Storage;

public class InMemoryBlobService : IBlobService
{
    private readonly ConcurrentDictionary<string, byte[]> _store = new();

    private static string Key(string container, string blob) => $"{container}/{blob}";

    public Task CreateContainerIfNotExistsAsync(string containerName, CancellationToken cancellationToken = default)
    {
        // No-op for in-memory
        return Task.CompletedTask;
    }

    public Task UploadAsync(string containerName, string blobName, Stream data, CancellationToken cancellationToken = default)
    {
        using var ms = new MemoryStream();
        data.CopyTo(ms);
        _store[Key(containerName, blobName)] = ms.ToArray();
        return Task.CompletedTask;
    }

    public Task DeleteIfExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        _store.TryRemove(Key(containerName, blobName), out _);
        return Task.CompletedTask;
    }

    public Task<Uri> GetBlobUriAsync(string containerName, string blobName)
    {
        // Return a fake URI for tests
        var uri = new Uri($"inmemory://{containerName}/{blobName}");
        return Task.FromResult(uri);
    }
}
