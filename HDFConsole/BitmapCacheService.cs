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

        public SKBitmap SetBitmap(string fileName, SKBitmap bitmap)
        {
            _cache.Set(fileName, bitmap, TimeSpan.FromMinutes(6));
            return _cache.Set("latest", bitmap, TimeSpan.FromMinutes(6));
        }

        public SKBitmap? GetBitmap(string fileName)
        {
            _cache.TryGetValue(fileName, out SKBitmap? bitmap);
            return bitmap;
        } 

        public byte[] SetImage(string fileName, byte[] imageData)
        {
            _cache.Set(fileName, imageData, TimeSpan.FromMinutes(6));
            return _cache.Set("latestImage", imageData, TimeSpan.FromMinutes(6));
        }

        public byte[] GetImage(string fileName)
        {
            _cache.TryGetValue(fileName, out byte[] image);
            return image;
        }
    }
}