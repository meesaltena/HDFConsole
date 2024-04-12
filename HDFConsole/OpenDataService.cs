using Microsoft.Extensions.Logging;
using System.Formats.Asn1;
using System.Net.Http;
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

        public async Task<OpenDataResponse?> GetRecentFiles(OpenDataDataSets datasetName, CancellationToken token = default(CancellationToken))
        {
            try
            {
                using Stream responseStream = await GetAsync("AuthorizedClient", datasetName, token, queryParam: "?sorting=desc");
                return await JsonSerializer.DeserializeAsync<OpenDataResponse>(responseStream, _jsonOptions, token);
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }
            return await Task.FromResult<OpenDataResponse?>(null);
        }

        public async Task<Stream?> DownloadFile(OpenDataDataSets datasetName, string fileName, CancellationToken token = default(CancellationToken))
        {
            try
            {          
                TemporaryDownloadUrlResponse? temporaryDownloadUrlResponse = await GetTemporaryDownloadUrl(datasetName, fileName, token);

                if (temporaryDownloadUrlResponse?.TemporaryDownloadUrl == null)
                {
                    _logger.LogError("Temporary download URL is null");
                    return null;
                }

                return await GetUriAsync("AnonymousClient", token, temporaryDownloadUrlResponse.TemporaryDownloadUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }

            return null;
        }

        private async Task<TemporaryDownloadUrlResponse?> GetTemporaryDownloadUrl(OpenDataDataSets datasetName, string fileName, CancellationToken token = default(CancellationToken))
        {
            try
            {
                using Stream responseStream = await GetAsync("AuthorizedClient", datasetName, token, $"{fileName}/url");
                return await JsonSerializer.DeserializeAsync<TemporaryDownloadUrlResponse>(responseStream, _jsonOptions, token);
            }
            catch (Exception ex)
            {
                _logger.LogError("OpenDataService Exception: {Error}", ex);
            }
            return null;
        }

        private async Task<Stream> GetUriAsync(string httpClientName, CancellationToken token = default(CancellationToken), string uri = "")
        {
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient(httpClientName);
                //TODO fix
                httpClient.BaseAddress = new Uri((uri ?? throw new ArgumentNullException()));
                return await httpClient.GetStreamAsync(uri, token);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<Stream> GetAsync(string httpClientName, OpenDataDataSets datasetName, CancellationToken token = default(CancellationToken), string path = "", string queryParam = "")
        {
            using HttpClient httpClient = _httpClientFactory.CreateClient(httpClientName);
            return await GetAsync(httpClient, datasetName, token, path, queryParam);
        }
           
        private async Task<Stream> GetAsync(HttpClient httpClient, OpenDataDataSets datasetName, CancellationToken token = default(CancellationToken), string path = "", string queryParam = "")
        {
            httpClient.BaseAddress = BuildUri(httpClient.BaseAddress, datasetName);
            return await httpClient.GetStreamAsync($"{path}{queryParam}", token);
        }

        private Uri BuildUri(Uri? inputUri, OpenDataDataSets datasetName)
        {
            if (!DatasetVersions.TryGetValue(datasetName, out var version))
            {
                _logger.LogWarning($"Dataset version not found for {datasetName}, assuming version 2");
                version = "2";
            }

            return new Uri((inputUri ?? throw new ArgumentNullException("InputUri not set"))
                .ToString()
                .Replace("{datasetName}", datasetName.ToString())
                .Replace("{version}", version));
        }

        private readonly Dictionary<OpenDataDataSets, string> DatasetVersions = new()
        {
            { OpenDataDataSets.Actuele10mindataKNMIstations, "2" },
            { OpenDataDataSets.radar_reflectivity_composites, "2.0" },
            { OpenDataDataSets.radar_forecast, "1.0" }
        };
    }
}