using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Xunit;
using Dapper;
using Microsoft.Data.Sqlite;

namespace PicHub.IntegrationTests
{
    public class AzuriteBlobTests
    {
        [Fact]
        public async Task UploadBlobAndInsertMetadata_Succeeds()
        {
            var connStr = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            Assert.False(string.IsNullOrEmpty(connStr), "AZURE_STORAGE_CONNECTION_STRING must be set to run integration test");

            var blobService = new BlobServiceClient(connStr);
            var container = blobService.GetBlobContainerClient("pichub-integration-test");
            await container.CreateIfNotExistsAsync();

            var testContent = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("hello-pichub"));
            var blobName = $"test/{Guid.NewGuid()}.txt";
            var blob = container.GetBlobClient(blobName);
            await blob.UploadAsync(testContent, overwrite: true);

            var exists = await blob.ExistsAsync();
            Assert.True(exists.Value, "Blob should exist after upload");

            // SQLite metadata check - lightweight mimic of repository persistence
            using var sqlite = new SqliteConnection("Data Source=:memory:");
            sqlite.Open();
            sqlite.Execute(@"CREATE TABLE MediaItems (Id TEXT PRIMARY KEY, AlbumId TEXT, Filename TEXT, ContentType TEXT, SizeBytes INTEGER, StoragePath TEXT, UploadedAt TEXT);");
            var id = Guid.NewGuid().ToString();
            sqlite.Execute("INSERT INTO MediaItems (Id, AlbumId, Filename, ContentType, SizeBytes, StoragePath, UploadedAt) VALUES (@Id,@AlbumId,@Filename,@ContentType,@SizeBytes,@StoragePath,@UploadedAt)", new
            {
                Id = id,
                AlbumId = Guid.NewGuid().ToString(),
                Filename = Path.GetFileName(blobName),
                ContentType = "text/plain",
                SizeBytes = 11,
                StoragePath = blobName,
                UploadedAt = DateTime.UtcNow.ToString("o")
            });

            var row = sqlite.QueryFirstOrDefault("SELECT * FROM MediaItems WHERE Id = @Id", new { Id = id });
            Assert.NotNull(row);
        }
    }
}
