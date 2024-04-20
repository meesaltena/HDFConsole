using HDFConsole.Models;

namespace HDFConsole.Services
{
    public class PeriodicFetcher : BackgroundService
    {
        private readonly ILogger<PeriodicFetcher> _logger;
        //private readonly OpenDataClient _openDataClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PeriodicFetcher(ILogger<PeriodicFetcher> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            //_openDataClient = openDataClient;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    OpenDataClient _openDataClient =
                        scope.ServiceProvider.GetRequiredService<OpenDataClient>();

                    //TODO just predict the filename to nearest 5 minutes, no need to GetRecentFiles from API
                    //await _openDataClient.GetMetaData(OpenDataDataSets.radar_reflectivity_composites, stoppingToken);
                    await _openDataClient.DownloadSaveAndCacheMostRecentFile(OpenDataDataSets.radar_reflectivity_composites, stoppingToken);
                    //await _openDataClient.DownloadSaveAndCacheMostRecentFile(OpenDataDataSets.Actuele10mindataKNMIstations, stoppingToken);

                    //await _openDataClient.DownloadAndCacheFiles(OpenDataDataSets.radar_forecast, stoppingToken);

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}