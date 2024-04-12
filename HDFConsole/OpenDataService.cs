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

        public async Task<OpenDataResponse?> GetRecentFiles(CancellationToken token = default(CancellationToken))
        {
            try
            {
                Stream responseStream = await httpClient.GetStreamAsync("", token);
                
                OpenDataResponse? res = await JsonSerializer.DeserializeAsync<OpenDataResponse>(responseStream,options,token);

                logger.LogInformation("OpenDataService got response with {0} files", res?.Files.Count);

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError("OpenDataService Exception: {Error}", ex);
            }
            return null;
        }

        public async Task<String?> DownloadFile(string fileName, CancellationToken token = default(CancellationToken))
        {
            try
            {
                TemporaryDownloadUrlResponse? temporaryDownloadUrlResponse = await GetTemporaryDownloadUrl(fileName, token);

                if(temporaryDownloadUrlResponse?.TemporaryDownloadUrl == null)
                {
                    logger.LogError("Temporary download URL is null");
                    return null;
                }   
               
                return await httpClient.GetStringAsync(temporaryDownloadUrlResponse.TemporaryDownloadUrl, token);
                
            }
            catch (Exception ex)
            {
                logger.LogError("OpenDataService Exception: {Error}", ex);
            }

            return null;
        }

        private async Task<TemporaryDownloadUrlResponse?> GetTemporaryDownloadUrl(string fileName, CancellationToken token = default(CancellationToken))
        {
            try
            {
                Stream responseStream = await httpClient.GetStreamAsync($"{fileName}/url", token);
                TemporaryDownloadUrlResponse? res = await JsonSerializer.DeserializeAsync<TemporaryDownloadUrlResponse>(responseStream, options, token);

                logger.LogInformation("OpenDataService got temporary dowload URL last modified at {0}", res?.LastModified);

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