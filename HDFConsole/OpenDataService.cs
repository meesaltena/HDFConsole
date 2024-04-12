using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HDFConsole
{
    public sealed class OpenDataService(
        HttpClient httpClient,
        ILogger<OpenDataService> logger) : IDisposable
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<OpenDataResponse> GetOpenDataResponse()
        {
            try
            {
                Stream responseStream = await httpClient.GetStreamAsync("");
                
                OpenDataResponse? res = await JsonSerializer.DeserializeAsync<OpenDataResponse>(responseStream,options);

                logger.LogInformation("OpenDataService got response with {0} files", res?.Files.Count);

                //return res ?? [];
                return res;
            }
            catch (Exception ex)
            {
                logger.LogError("OpenDataService Exception: {Error}", ex);
            }

            return null;
        }

        public void Dispose() => httpClient?.Dispose();
    }
}