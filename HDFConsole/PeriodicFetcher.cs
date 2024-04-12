using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Formats.Asn1;
using System.Net.Http;
using System.Text.Json;

namespace HDFConsole
{
    public class PeriodicFetcher : BackgroundService
    {
        private readonly ILogger<PeriodicFetcher> _logger;
        private readonly OpenDataClient _openDataClient;

        public PeriodicFetcher(ILogger<PeriodicFetcher> logger, OpenDataClient openDataClient)
        {
            _logger = logger;
            _openDataClient = openDataClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _openDataClient.DownloadMostRecentFile(OpenDataDataSets.radar_reflectivity_composites);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}