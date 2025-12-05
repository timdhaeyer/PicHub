namespace PicHub.AlbumUploader.Models;

public class PublicAlbumDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool AllowUploads { get; set; }
    public int? MaxFileSizeMb { get; set; }
    public string AlbumSizeTshirt { get; set; } = "M";
    public IEnumerable<MediaItem> Items { get; set; } = Array.Empty<MediaItem>();
}
