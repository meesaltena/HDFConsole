using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace HDFConsole.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ImageController(IServiceScopeFactory serviceScopeFactory )
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        [Route("latest")]
        public ActionResult<string> GetLatestImage()
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                BitmapCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<BitmapCacheService>();
                byte[] image = _bitmapCache.GetImage("latestImage") ?? throw new ArgumentNullException();
                if (image == null)
                {
                    return "err";
                }
                return File(image, "image/png");
            }
        }
    }
}
