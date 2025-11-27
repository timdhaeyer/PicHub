using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services;

public class QuotaService
{
    public static bool CanUpload(Album album, long newFileSizeBytes)
    {
        if (!album.AllowUploads)
            return false;

        var newTotal = album.TotalBytesUsed + newFileSizeBytes;
        long capBytes = album.AlbumSizeTshirt switch
        {
            "XS" => 1L * 1024 * 1024 * 1024,
            "S" => 5L * 1024 * 1024 * 1024,
            "M" => 10L * 1024 * 1024 * 1024,
            "L" => 50L * 1024 * 1024 * 1024,
            "XL" => 100L * 1024 * 1024 * 1024,
            _ => 10L * 1024 * 1024 * 1024
        };

        return newTotal <= capBytes;
    }
}
