using HDFConsole.Models;
using HDFConsole.Services;
using Microsoft.Extensions.Options;
using PureHDF;
using SkiaSharp;

namespace HDFConsole
{
    public sealed class OpenDataClient
    {
        private readonly OpenDataService _openDataService = null!;
        private readonly ILogger<OpenDataClient> _logger = null!;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly OpenDataClientOptions _options;

        public OpenDataClient(OpenDataService openDataService, ILogger<OpenDataClient> logger, IServiceScopeFactory serviceScopeFactory,
            IOptions<OpenDataClientOptions> options)
        {
            _openDataService = openDataService;
            _logger = logger;
            _options = options.Value;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task DownloadSaveAndCacheMostRecentFile(OpenDataDataSets dataset, CancellationToken cancellationToken = default)
        {
            var response = await _openDataService.GetRecentFilesAsync(dataset, cancellationToken);
            HDFFile? file = response?.Files.OrderByDescending(r => r.LastModified).FirstOrDefault();

            if (file == null)
            {
                _logger.LogWarning($"Got file null response from OpenDataService");
                return;
            }

            string fullPath = $"{GetDownloadDirectory()}{file.Filename}";

            try
            {
                var stream = await _openDataService.DownloadFileStreamAsync(dataset, file.Filename, cancellationToken);
                if (stream == null) return;
              
                file.imageData = await GetImageBytes(stream);
                CacheLatestHDFFile(file);
                await SaveToFile(stream, fullPath); // save .H5 to file


                string pngFilename = fullPath.Replace(".h5", "_image.png");
                await SaveToFile(new MemoryStream(file.imageData), pngFilename); // save .PNG to file
 
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DownloadMostRecentFile caught exception:");
                throw;
            }
        }

        private async Task SaveToFile(Stream stream, string fullPath, CancellationToken token = default)
        {
            using FileStream fileStream = new(fullPath, FileMode.Create);
            await stream.CopyToAsync(fileStream, token);

            _logger.LogInformation($"{DateTime.Now} Wrote file to {fullPath}");
        }

        private void CacheLatestHDFFile(HDFFile? file)
        {
            if (file == null) return;

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                _bitmapCache.SetFile(file, "latestFile");
            }
            _logger.LogInformation($"{DateTime.Now} Cached File {file.Filename} with key:latestFile");
        }

        private async Task<byte[]> GetImageBytes(Stream inputStream, CancellationToken token = default)
        {
            using (var stream = new MemoryStream())
            {
                await inputStream.CopyToAsync(stream, token);
                stream.Position = 0;

                using var hdfFile = H5File.Open(stream, false);
                byte[,] imageData = hdfFile.Group("/image1").Dataset("image_data").Read<byte[,]>();

                int width = imageData.GetLength(1);
                int height = imageData.GetLength(0);

                byte[,] colorPalette = hdfFile.Group("/visualisation1").Dataset("color_palette").Read<byte[,]>(); // 256x3

                using SKBitmap bitmap = new(width, height);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte val = imageData[y, x];
                        if (val == 255 || val == 0)
                        {
                            bitmap.SetPixel(x, y, SKColor.Parse("#000000"));
                        }
                        else
                        { 
                            int r = colorPalette[val, 0];
                            int g = colorPalette[val, 1];
                            int b = colorPalette[val, 2];
                            var hex = string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
                            bitmap.SetPixel(x, y, SKColor.Parse(hex));
                        }
                    }
                }
                // TODO just return bitmap.Bytes?
                // TODO using?
                using SKImage image = SKImage.FromBitmap(bitmap);
                using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
                return encoded.ToArray();
            }
        }

        private string GetDownloadDirectory()
        {
            string path;
            if (_options.Absolute)
            {
                path = Path.GetFullPath(_options.DownloadDirectory);
            }
            else
            {
                path = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}{_options.DownloadDirectory}";
            }
            Directory.CreateDirectory(path);
          

            if (string.IsNullOrWhiteSpace(_options.DownloadDirectory))
            {
                return path;
            }

            return $"{path}{Path.DirectorySeparatorChar}";
        }
    }
}