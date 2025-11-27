using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using AlbumUploader.Models;

namespace AlbumUploader.Services
{
    public class AlbumRepository
    {
        private readonly string _connectionString;

        public AlbumRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection() => new SqlConnection(_connectionString);

        public Album? GetByPublicToken(string publicToken)
        {
            using var db = Connection();
            return db.QueryFirstOrDefault<Album>("SELECT TOP 1 * FROM Albums WHERE PublicToken = @Token", new { Token = publicToken });
        }

        public void InsertMediaItem(MediaItem item)
        {
            using var db = Connection();
            db.Execute("INSERT INTO MediaItems (Id, AlbumId, Filename, ContentType, SizeBytes, StoragePath, ThumbnailPath, UploadedAt, UploadedBy, IsProcessed) VALUES (@Id,@AlbumId,@Filename,@ContentType,@SizeBytes,@StoragePath,@ThumbnailPath,@UploadedAt,@UploadedBy,@IsProcessed)", item);
            db.Execute("UPDATE Albums SET TotalBytesUsed = TotalBytesUsed + @Size WHERE Id = @AlbumId", new { Size = item.SizeBytes, AlbumId = item.AlbumId });
        }
    }
}
