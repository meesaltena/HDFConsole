using Microsoft.AspNetCore.Mvc;

namespace HDFConsole.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        [Route("latest")]
        public ActionResult<string> GetLatestImage()
        {
            var file = @"";
            return File(System.IO.File.ReadAllBytes(file), "image/png");
        }
    }
}
