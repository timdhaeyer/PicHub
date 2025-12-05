using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Xunit;
using Microsoft.Data.Sqlite;
using Dapper;

namespace PicHub.IntegrationTests
{
    public class AlbumUploadIntegrationTests
    {
        [Fact]
        public async Task UploadMedia_EndToEnd_With_AzuriteAndSql()
        {
            var connStr = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connStr))
            {
                Console.WriteLine("AZURE_STORAGE_CONNECTION_STRING not set; skipping integration test");
                return;
            }

            var sqlConn = Environment.GetEnvironmentVariable("SqlConnectionString");
            if (string.IsNullOrEmpty(sqlConn))
            {
                Console.WriteLine("SqlConnectionString not set; skipping integration test");
                return;
            }

            var blobService = new BlobServiceClient(connStr);
            var container = blobService.GetBlobContainerClient("pichub-integration-test");
            await container.CreateIfNotExistsAsync();

            using (var testContent = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("hello-pichub-integration")))
            {
                var blobName = $"test/{Guid.NewGuid()}.txt";
                var blob = container.GetBlobClient(blobName);
                await blob.UploadAsync(testContent, overwrite: true);
            }

            var exists = await blob.ExistsAsync();
            Assert.True(exists.Value, "Blob should exist after upload");

            // Very small SQL metadata insert to validate DB connectivity
            using var sqlite = new SqliteConnection(sqlConn);
            sqlite.Open();
            sqlite.Execute(@"CREATE TABLE IF NOT EXISTS MediaItems (Id TEXT PRIMARY KEY, AlbumId TEXT, Filename TEXT, ContentType TEXT, SizeBytes INTEGER, StoragePath TEXT, UploadedAt TEXT);");
            var id = Guid.NewGuid().ToString();
            sqlite.Execute("INSERT INTO MediaItems (Id, AlbumId, Filename, ContentType, SizeBytes, StoragePath, UploadedAt) VALUES (@Id,@AlbumId,@Filename,@ContentType,@SizeBytes,@StoragePath,@UploadedAt)", new
            {
                Id = id,
                AlbumId = Guid.NewGuid().ToString(),
                Filename = Path.GetFileName(blobName),
                ContentType = "text/plain",
                SizeBytes = 16,
                StoragePath = blobName,
                UploadedAt = DateTime.UtcNow.ToString("o")
            });

            var row = sqlite.QueryFirstOrDefault("SELECT * FROM MediaItems WHERE Id = @Id", new { Id = id });
            Assert.NotNull(row);
        }
    }
}
