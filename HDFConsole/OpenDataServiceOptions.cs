namespace HDFConsole
{
    public class OpenDataServiceOptions
    {
        public OpenDataServiceOptions()
        {

        }
        public string ApiKey { get; set; } = string.Empty;
        public string BaseAddress { get; set; } = string.Empty;
        public Dictionary<string, string> DatasetVersions { get; set; } = new();
    }
}