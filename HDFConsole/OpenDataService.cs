using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HDFConsole
{
    public class OpenDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OpenDataService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly OpenDataServiceOptions _options;
        public OpenDataService(IHttpClientFactory httpClientFactory, ILogger<OpenDataService> logger, IOptions<OpenDataServiceOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options.Value;
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }
        public async Task<OpenDataResponse?> GetRecentFiles(OpenDataDataSets datasetName, CancellationToken cancellationToken = default)
        {
            try
            {
                string baseUri = BuildDatasetRequestBaseUri(datasetName);

                using HttpClient httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_options.ApiKey);

                using Stream responseStream = await httpClient.GetStreamAsync($"{baseUri}?sorting=desc", cancellationToken);

                return await JsonSerializer.DeserializeAsync<OpenDataResponse>(responseStream, _jsonOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }
            return await Task.FromResult<OpenDataResponse?>(null);
        }

        public async Task<Stream?> DownloadFile(OpenDataDataSets datasetName, string fileName, CancellationToken token = default)
        {
            try
            {          
                TemporaryDownloadUrlResponse? temporaryDownloadUrlResponse = await GetTemporaryDownloadUrl(datasetName, fileName, token);

                if (temporaryDownloadUrlResponse?.TemporaryDownloadUrl == null)
                {
                    _logger.LogError("Temporary download URL is null");
                    return null;
                }

                using HttpClient httpClient = _httpClientFactory.CreateClient();
                return await httpClient.GetStreamAsync(temporaryDownloadUrlResponse.TemporaryDownloadUrl, token);
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }

            return null;
        }

        private async Task<TemporaryDownloadUrlResponse?> GetTemporaryDownloadUrl(OpenDataDataSets datasetName, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                string baseUri = BuildDatasetRequestBaseUri(datasetName);

                using HttpClient httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_options.ApiKey);      
                
                using Stream responseStream = await httpClient.GetStreamAsync($"{baseUri}{fileName}/url", cancellationToken);

                return await JsonSerializer.DeserializeAsync<TemporaryDownloadUrlResponse>(responseStream, _jsonOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }
            return null;
        }

        private string BuildDatasetRequestBaseUri(OpenDataDataSets datasetName)
        {
            var version = _options.DatasetVersions.FirstOrDefault(d => d.Key.Contains(datasetName.ToString())).Value;

            if (string.IsNullOrWhiteSpace(version))
            {
                _logger.LogError($"Dataset version not found for {datasetName}, assuming version 2");
                version = "2";
            }

            return $"{_options.BaseAddress}"
                .Replace("{datasetName}", datasetName.ToString())
                .Replace("{version}", version);
        }
    }
}