using HDFConsole.Models;
using HDFConsole.Services;
using Microsoft.AspNetCore.Mvc;

namespace HDFConsole.Controllers
{
    [ApiController]
    [Route("")]
    public class ImageController : Controller
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ImageController(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        // radar_reflectivity_composites
        // radar_forecast
        // Actuele10mindataKNMIstations


        [Route("")]
        public IActionResult GetImage([FromQuery] string datasetName = "radar_reflectivity_composites")
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                HDFFile? file = _bitmapCache.GetFile(datasetName);

                if (file == null)
                {
                    return Content("<p>Error: Image or file null.</p>", "text/html");
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
                List<HDFFile> files = _bitmapCache.GetFiles($"{datasetName}List");

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
