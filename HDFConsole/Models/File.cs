public class File
{
    public string Filename { get; set; } = null!;
    public int Size { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
}