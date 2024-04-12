public class TemporaryDownloadUrlResponse
{
    public string ContentType { get; set; } = null!;
    public DateTime LastModified { get; set; }
    public string Size { get; set; } = null!;
    public string TemporaryDownloadUrl { get; set; } = null!;
}
