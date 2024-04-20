namespace HDFConsole.Models
{
    public class OpenDataServiceOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string EDRBaseAddress { get; set; } = string.Empty;
        public string OpenDataBaseAddress { get; set; } = string.Empty;
        public string MetaDataBaseAddress { get; set; } = string.Empty;
        public Dictionary<string, string> EDRDatasetVersions { get; set; } = new();
        public Dictionary<string, string> OpenDataDatasetVersions { get; set; } = new();
    }
}