namespace HDFConsole.Models
{
    public class OpenDataResponse
    {
        public bool IsTruncated { get; set; }
        public int ResultCount { get; set; }
        public List<HDFFile> Files { get; set; } = [];
        public int MaxResults { get; set; }
        public string StartAfterFilename { get; set; } = null!;
        public string NextPageToken { get; set; } = null!;
    }
}