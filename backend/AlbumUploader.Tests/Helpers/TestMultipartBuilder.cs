using System.Text;

namespace PicHub.AlbumUploader.Tests.Helpers
{
    public static class TestMultipartBuilder
    {
        // Builds a minimal multipart/form-data payload using explicit CRLF sequences.
        public static (string ContentType, byte[] Body) BuildSingleFile(string fieldName, string fileName, string contentType, string content)
        {
            var boundary = "----WebKitFormBoundary7MA4YWxkTrZu0gW";
            var sb = new StringBuilder();
            sb.Append($"--{boundary}")
                .Append("\r\n")
                .Append($"Content-Disposition: form-data; name=\"{fieldName}\"; filename=\"{fileName}\"")
                .Append("\r\n")
                .Append($"Content-Type: {contentType}")
                .Append("\r\n\r\n")
                .Append(content)
                .Append("\r\n")
                .Append($"--{boundary}--")
                .Append("\r\n");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var contentTypeHeader = $"multipart/form-data; boundary={boundary}";
            return (contentTypeHeader, bytes);
        }
    }
}
