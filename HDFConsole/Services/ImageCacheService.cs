using HDFConsole.Models;

using Microsoft.Extensions.Caching.Memory;
namespace HDFConsole.Services
{

    public class ImageCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<ImageCacheService> _logger;

        public ImageCacheService(IMemoryCache cache, ILogger<ImageCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public HDFFile? GetFile(string fileName)
        {
            _cache.TryGetValue(fileName, out HDFFile? file);
            return file;
        }

        public HDFFile SetFile(HDFFile file, string filename)
        {
            return _cache.Set(filename, file, TimeSpan.FromMinutes(6));
        }
    }
}