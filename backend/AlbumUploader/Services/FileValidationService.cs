using System.Collections.Generic;
using System.Linq;

namespace AlbumUploader.Services
{
    public class FileValidationService
    {
        private static readonly HashSet<string> _allowedContentTypes = new HashSet<string>
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "video/mp4",
            "video/webm"
        };

        public bool IsContentTypeAllowed(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType)) return false;
            return _allowedContentTypes.Contains(contentType.ToLowerInvariant());
        }

        public bool IsUnderMaxSize(long bytes, int maxFileSizeMb)
        {
            var maxBytes = (long)maxFileSizeMb * 1024 * 1024;
            return bytes <= maxBytes;
        }
    }
}
