namespace PicHub.AlbumUploader.Services;

public class FileValidationService
{
    private static readonly HashSet<string> _allowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "video/mp4",
        "video/webm"
    ];

    public static bool IsContentTypeAllowed(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return false;
        return _allowedContentTypes.Contains(contentType.ToLowerInvariant());
    }

    public static bool IsUnderMaxSize(long bytes, int maxFileSizeMb)
    {
        var maxBytes = (long)maxFileSizeMb * 1024 * 1024;
        return bytes <= maxBytes;
    }
}
