using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Xunit;
using Microsoft.Data.Sqlite;
using Dapper;

namespace PicHub.IntegrationTests
{
    public class AlbumExportDeleteIntegrationTests
    {
        [Fact]
        public async Task ExportAndDelete_EndToEnd_With_AzuriteAndSql()
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

            // Upload a blob that represents exported ZIP
            var testContent = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("zip-content"));
            var blobName = $"exports/{Guid.NewGuid()}.zip";
            var blob = container.GetBlobClient(blobName);
            await blob.UploadAsync(testContent, overwrite: true);

            var exists = await blob.ExistsAsync();
            Assert.True(exists.Value, "Export blob should exist after upload");

            // Delete blob
            await blob.DeleteIfExistsAsync();
            var existsAfterDelete = await blob.ExistsAsync();
            Assert.False(existsAfterDelete.Value, "Blob should not exist after delete");

            // Minimal SQL check
            using var sqlite = new SqliteConnection(sqlConn);
            sqlite.Open();
            sqlite.Execute(@"CREATE TABLE IF NOT EXISTS ExportJobs (Id TEXT PRIMARY KEY, AlbumId TEXT, Path TEXT, CreatedAt TEXT);");
            var id = Guid.NewGuid().ToString();
            sqlite.Execute("INSERT INTO ExportJobs (Id, AlbumId, Path, CreatedAt) VALUES (@Id,@AlbumId,@Path,@CreatedAt)", new
            {
                Id = id,
                AlbumId = Guid.NewGuid().ToString(),
                Path = blobName,
                CreatedAt = DateTime.UtcNow.ToString("o")
            });

            var row = sqlite.QueryFirstOrDefault("SELECT * FROM ExportJobs WHERE Id = @Id", new { Id = id });
            Assert.NotNull(row);
        }
    }
}
