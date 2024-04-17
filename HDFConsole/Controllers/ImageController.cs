using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

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

        [Route("image")]
        public ActionResult<string> GetLatestImage()
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                byte[] image = _bitmapCache.GetImage("latestImage") ?? throw new ArgumentNullException();
                if (image == null)
                {
                    return "err";
                }
                return File(image, "image/png");
                }
        }

        [Route("")]
        public IActionResult GetImage([FromQuery] string message = "Hello")
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                byte[] image = _bitmapCache.GetImage("latestImage");
                File file = _bitmapCache.GetFile("latestFile");

                if (image == null || file == null)
                {
                    return Content("<p>Error: Image or file null.</p>", "text/html");
                }
                var base64String = Convert.ToBase64String(image);
                var imgSrc = $"data:image/png;base64,{base64String}";
                TimeSpan diff = DateTime.Now.Subtract(file.Created);
                ViewData["Message"] = $"Latest image: {file.Filename}, Created: {file.Created}, ({ToRelativeTime(diff)}), Size: {file.Size/1000}KB";
                ViewData["latestImage"] = imgSrc;
                return View("Image");
            }
        }

        private string ToRelativeTime(TimeSpan diff) => $"{(int)diff.TotalMinutes}m{(int)diff.Seconds}s ago";
    }
}
