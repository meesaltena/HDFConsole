using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;

namespace HDFConsole
{
    public sealed class OpenDataClient
    {
        private readonly OpenDataService _openDataService = null!;
        private readonly ILogger<OpenDataClient> _logger = null!;
        public OpenDataClient(OpenDataService openDataService, ILogger<OpenDataClient> logger)
        {
            _openDataService = openDataService;
            _logger = logger;
        }

        public async Task DownloadMostRecentFile(OpenDataDataSets dataset)
        {
            var response = await _openDataService.GetRecentFiles(dataset);
            var file = response?.Files.OrderByDescending(r => r.LastModified).FirstOrDefault();

            if (file == null)
            {
                _logger.LogWarning($"Got file null response from OpenDataService");
                return;
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            string fullPath = @$"{currentDirectory}\{file.Filename}";

            using Stream stream = await _openDataService.DownloadFile(dataset, file.Filename);
            using FileStream fileStream = new(fullPath, FileMode.Create);

            await stream.CopyToAsync(fileStream);
            _logger.LogInformation($"Wrote {file.Filename} to file {fullPath}");
        }
    }
}
