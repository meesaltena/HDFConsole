using HDFConsole.Models;
using HDFConsole.Models.Enums;
using HDFConsole.Services;
using Microsoft.AspNetCore.Mvc;

namespace HDFConsole.Controllers
{
    [ApiController]
    [Route("")]
    public class ImageController : Controller
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ImageController> _logger;
        public ImageController(IServiceScopeFactory serviceScopeFactory, ILogger<ImageController> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

        }

        // radar_reflectivity_composites
        // radar_forecast
        // Actuele10mindataKNMIstations


        [Route("")]
        public async Task<IActionResult> GetImage([FromQuery] string datasetName = "radar_reflectivity_composites")
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                HDFFile? file = _bitmapCache.GetFile(datasetName);

                if (file == null)
                {
                    _logger.LogInformation("Files cache miss, downloading and caching...");
                    OpenDataClient _openDataClient =
                        scope.ServiceProvider.GetRequiredService<OpenDataClient>();

                    if (!Enum.TryParse(datasetName, out OpenDataDataSets dataset))
                        return Content($"<p>Error: Failed to parse enum {datasetName} or file null.</p>", "text/html");

                    await _openDataClient.DownloadAndCacheFiles(dataset);

                    var result = _bitmapCache.GetFiles($"{datasetName}List"); 
                    if(result == null) return Content("<p>Error: Image or file null.</p>", "text/html");
                    file = result.First();
                }
                ViewData.Model = file;
                return View("Image");
            }
        }

        [Route("latest")]
        public HDFFile? GetLatestImage([FromQuery] string datasetName = "radar_reflectivity_composites")
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateAsyncScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                return _bitmapCache.GetFile(datasetName);             
            }
        }

        [Route("list")]
        public IActionResult GetImageList([FromQuery] string datasetName = "radar_forecast")
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                List<HDFFile>? files = _bitmapCache.GetFiles($"{datasetName}List");

                if (files == null)
                {
                    return Content("<p>Error: Image or file null.</p>", "text/html");
                }

                ViewData.Model = files;
                return View("ImageList");
            }
        }


        //private string ToRelativeTime(TimeSpan diff) => $"{(int)diff.TotalMinutes}m{(int)diff.Seconds}s ago";
    }
}
