using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;

namespace HDFConsole
{

    public class BitmapCacheService
    {
        private readonly IMemoryCache _cache;

        public BitmapCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public byte[] SetImage(byte[] imageData)
        {
            return SetImage("latest", imageData);
        }

        public byte[] SetImage(string fileName, byte[] imageData)
        {
            return _cache.Set(fileName, imageData, TimeSpan.FromMinutes(6));
        }

        public byte[]? GetImage()
        {
            return GetImage("latest");
        }

        public byte[]? GetImage(string fileName)
        {
            _cache.TryGetValue(fileName, out byte[]? image);
            return image;
        }
    }
}