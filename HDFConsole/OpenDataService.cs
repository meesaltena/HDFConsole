using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HDFConsole
{
    public class OpenDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OpenDataService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public OpenDataService(IHttpClientFactory httpClientFactory, ILogger<OpenDataService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<OpenDataResponse?> GetRecentFiles(CancellationToken token = default(CancellationToken))
        {
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient("AuthorizedClient");

                using Stream responseStream = await httpClient.GetStreamAsync("", token);
                
                OpenDataResponse? res = await JsonSerializer.DeserializeAsync<OpenDataResponse>(responseStream,_jsonOptions,token);

                _logger.LogInformation("OpenDataService got response with {0} files", res?.Files.Count);

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }
            return null;
        }

        public async Task<Stream> DownloadFile(string fileName, CancellationToken token = default(CancellationToken))
        {
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient("AnonymousClient");
                TemporaryDownloadUrlResponse? temporaryDownloadUrlResponse = await GetTemporaryDownloadUrl(fileName, token);

                if(temporaryDownloadUrlResponse?.TemporaryDownloadUrl == null)
                {
                    _logger.LogError("Temporary download URL is null");
                    return await Task.FromResult<Stream>(Stream.Null);
                }   
               
                return await httpClient.GetStreamAsync(temporaryDownloadUrlResponse.TemporaryDownloadUrl, token);
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }

             return await Task.FromResult<Stream>(Stream.Null);
        }

        private async Task<TemporaryDownloadUrlResponse?> GetTemporaryDownloadUrl(string fileName, CancellationToken token = default(CancellationToken))
        {
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient("AuthorizedClient");
                using Stream responseStream = await httpClient.GetStreamAsync($"{fileName}/url", token);
                TemporaryDownloadUrlResponse? res = await JsonSerializer.DeserializeAsync<TemporaryDownloadUrlResponse>(responseStream, _jsonOptions, token);

                _logger.LogInformation("OpenDataService got temporary dowload URL last modified at {0}", res?.LastModified);

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }
            return await Task.FromResult<TemporaryDownloadUrlResponse?>(null);
        }
    }
}