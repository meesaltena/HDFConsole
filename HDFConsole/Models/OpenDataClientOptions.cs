namespace HDFConsole.Models
{
    public class OpenDataClientOptions
    {
        /// <summary>
        /// Directory to download images and files; Relative to Directory.GetCurrentDirectory() when Absolute == false
        /// </summary>
        public string DownloadDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Treat DownloadDirectory as absolute, relative to Directory.GetCurrentDirectory() when false
        /// </summary>
        public bool Absolute { get; set; }

    }
}