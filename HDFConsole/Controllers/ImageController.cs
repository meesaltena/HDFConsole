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
                //byte[] image = _bitmapCache.GetImage("latestImage");
                HDFFile file = _bitmapCache.GetFile(datasetName);

                if (file == null)
                {
                    return Content("<p>Error: Image or file null.</p>", "text/html");
                }

                //var base64String = Convert.ToBase64String(file.ImageData);
                //ViewData["Message"] = $"Latest image: {file.Filename}, Created: {file.Created}, ({ToRelativeTime(diff)}), Size: {file.Size/1000}KB";
                //ViewData["latestImage"] = imgSrc;
                //var imgSrc = $"data:image/png;base64,{base64String}";
                //TimeSpan diff = DateTime.Now.Subtract(file.Created);
                ViewData.Model = file;
 
                return View("Image");
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
