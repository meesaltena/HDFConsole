public class OpenDataResponse
{
    public bool IsTruncated { get; set; }
    public int ResultCount { get; set; }
    public List<File> Files { get; set; }
    public int MaxResults { get; set; }
    public string StartAfterFilename { get; set; }
    public string NextPageToken { get; set; }
}
