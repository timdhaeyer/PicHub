namespace AlbumUploader.Models
{
    public class Album
    {
        public System.Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string PublicToken { get; set; } = string.Empty;
        public bool AllowUploads { get; set; }
        public string AlbumSizeTshirt { get; set; } = "M";
        public long TotalBytesUsed { get; set; }
        public int? MaxFileSizeMb { get; set; }
    }
}
