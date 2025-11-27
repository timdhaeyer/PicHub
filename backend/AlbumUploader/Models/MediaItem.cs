namespace AlbumUploader.Models
{
    public class MediaItem
    {
        public System.Guid Id { get; set; }
        public System.Guid AlbumId { get; set; }
        public string Filename { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string? ContentType { get; set; }
        public string? StoragePath { get; set; }
        public System.DateTime UploadedAt { get; set; }
        public bool IsProcessed { get; set; }
    }
}
