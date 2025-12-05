namespace PicHub.AlbumUploader.Models;

public class CreateAlbumRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool AllowUploads { get; set; } = true;
    public int? MaxFileSizeMb { get; set; }
    public string? AlbumSizeTshirt { get; set; }
    public int? RetentionDays { get; set; }
}
