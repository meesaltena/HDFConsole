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


        [Route("")]
        public IActionResult GetImage([FromQuery] string message = "Hello")
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                //byte[] image = _bitmapCache.GetImage("latestImage");
                HDFFile file = _bitmapCache.GetFile("latestFile");

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

        //private string ToRelativeTime(TimeSpan diff) => $"{(int)diff.TotalMinutes}m{(int)diff.Seconds}s ago";
    }
}
