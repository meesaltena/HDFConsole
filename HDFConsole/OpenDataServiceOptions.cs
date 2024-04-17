namespace HDFConsole
{
    public class OpenDataServiceOptions
    {
        public OpenDataServiceOptions()
        {

        }
        public string ApiKey { get; set; } = string.Empty;
        public string EDRBaseAddress { get; set; } = string.Empty;
        public string OpenDataBaseAddress { get; set; } = string.Empty;
        public Dictionary<string, string> EDRDatasetVersions { get; set; } = new();
        public Dictionary<string, string> OpenDataDatasetVersions { get; set; } = new();
    }
}