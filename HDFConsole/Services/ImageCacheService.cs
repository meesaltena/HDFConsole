using HDFConsole.Models;

using Microsoft.Extensions.Caching.Memory;
namespace HDFConsole.Services
{

    //TODO thread safety
    public class ImageCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<ImageCacheService> _logger;

        public ImageCacheService(IMemoryCache cache, ILogger<ImageCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public HDFFile? GetFile(string key)
        {
            _cache.TryGetValue(key, out HDFFile? file);
            return file;
        }

        public List<HDFFile> GetFiles(string key)
        {
            _cache.TryGetValue(key, out List<HDFFile>? files);
            return files;
        }

        public HDFFile SetFile(HDFFile file, string key)
        {
            return _cache.Set(key, file, TimeSpan.FromMinutes(6));
        }

        public List<HDFFile> SetFiles(List<HDFFile> files, string key)
        {
            return _cache.Set(key, files, TimeSpan.FromMinutes(6));
        }
    }
}