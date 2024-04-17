using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;

namespace HDFConsole
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

        public File? GetFile(string fileName)
        {
            _cache.TryGetValue(fileName, out File? file);
            return file;
        }

        public File SetFile(File file, string filename)
        {
            return _cache.Set(filename, file, TimeSpan.FromMinutes(6));
        }

        //TODO ReadOnlySpan?
        public byte[] SetImage(string fileName, byte[] imageData)
        {
            return _cache.Set(fileName, imageData, TimeSpan.FromMinutes(6));
        }

        public byte[]? GetImage(string fileName)
        {
            if (!_cache.TryGetValue(fileName, out byte[]? image))
                _logger.LogError($"Image cache miss : {fileName}");
            return image;
        }
    }
}