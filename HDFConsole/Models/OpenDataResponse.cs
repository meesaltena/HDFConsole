public class File
{
    public string Filename { get; set; }
    public int Size { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
}

public class Response
{
    public bool IsTruncated { get; set; }
    public int ResultCount { get; set; }
    public List<File> Files { get; set; }
    public int MaxResults { get; set; }
    public string StartAfterFilename { get; set; }
    public string NextPageToken { get; set; }
}
