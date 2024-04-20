using HDFConsole.Models;
using HDFConsole.Models.Enums;
using HDFConsole.Services;
using Microsoft.AspNetCore.Mvc;

namespace HDFConsole.Controllers
{
    [ApiController]
    [Route("files")]
    public class FileController : Controller
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<FileController> _logger;

        public FileController(IServiceScopeFactory serviceScopeFactory, ILogger<FileController> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

        }

        [Route("list")]
        public async Task<List<HDFFile>> GetFileList([FromQuery] string datasetName = "radar_forecast")
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ImageCacheService _bitmapCache =
                scope.ServiceProvider.GetRequiredService<ImageCacheService>();
            List<HDFFile> files = _bitmapCache.GetFiles($"{datasetName}List");

            if(files == null || files.Count == 0)
            {
                _logger.LogInformation("Files cache miss, downloading and caching...");
;                OpenDataClient _openDataClient =
                    scope.ServiceProvider.GetRequiredService<OpenDataClient>();
               
                await _openDataClient.DownloadAndCacheFiles(OpenDataDataSets.radar_forecast);
                files = _bitmapCache.GetFiles($"{datasetName}List");
            }

            return files;
        }
    }
}
