namespace PicHub.AlbumUploader.Models;

public class CreateAlbumRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
