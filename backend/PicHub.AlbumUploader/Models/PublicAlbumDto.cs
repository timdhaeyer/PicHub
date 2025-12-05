namespace PicHub.AlbumUploader.Models;

public class MediaItemDto
{
    public Guid Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string? ContentType { get; set; }
    public string? StoragePath { get; set; }
    public DateTime UploadedAt { get; set; }
    public Uri? BlobUri { get; set; }
}

public class PublicAlbumDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool AllowUploads { get; set; }
    public int? MaxFileSizeMb { get; set; }
    public string AlbumSizeTshirt { get; set; } = "M";
    public IEnumerable<MediaItemDto> Items { get; set; } = Array.Empty<MediaItemDto>();
}
