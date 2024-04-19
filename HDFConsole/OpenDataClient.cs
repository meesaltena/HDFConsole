using HDFConsole.Models;
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

        public async Task DownloadMostRecentFile(OpenDataDataSets dataset, CancellationToken cancellationToken = default)
        {
            var response = await _openDataService.GetRecentFilesAsync(dataset, cancellationToken);
            var file = response?.Files.OrderByDescending(r => r.LastModified).FirstOrDefault();

            if (file == null)
            {
                _logger.LogWarning($"Got file null response from OpenDataService");
                return;
            }

            string fullPath = $"{GetDownloadDirectory()}{file.Filename}";

            try
            {
                using var stream = await _openDataService.DownloadFileStreamAsync(dataset, file.Filename, cancellationToken);
                if (stream != null)
                {
                    using FileStream fileStream = new(fullPath, FileMode.Create);
                    await stream.CopyToAsync(fileStream, cancellationToken);
                    CacheLatestFileInfo(file);
                }
                else
                {
                    _logger.LogError($"Filestream null for file {fullPath}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DownloadMostRecentFile caught exception:");
                throw;
            }

            _logger.LogInformation($"{DateTime.Now} Wrote {file.Filename} to file {fullPath}");
            ReadH5FileAndSaveAsBitMap(fullPath);
        }

        private void CacheLatestFileInfo(File? file)
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

        private readonly SKColor[] infernoPalette =
        [
        new SKColor(20, 11, 52),
        new SKColor(132, 32, 107),
        new SKColor(229, 92, 48),
        new SKColor(246, 215, 70)
        ];

        private void ReadH5FileAndSaveAsBitMap(string fileName)
        {
            var hdfFile = H5File.OpenRead(fileName);
            var imageGroup = hdfFile.Group("/image1");
            var imageData = imageGroup.Dataset("image_data").Read<byte[,]>();

            int width = imageData.GetLength(1);
            int height = imageData.GetLength(0);

            using SKBitmap bitmap = new(width, height);
            var vizGroup = hdfFile.Group("/visualisation1");
            byte[,] colorPalette = vizGroup.Dataset("color_palette").Read<byte[,]>(); // 256x3 ;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SKColor color;
                    byte val = imageData[y, x];
                    if (val == 255 || val == 0)
                    {
                        color = SKColor.Parse("#000000");
                    }
                    else
                    {
                        int r = colorPalette[val, 0];
                        int g = colorPalette[val, 1];
                        int b = colorPalette[val, 2];
                        var hex = string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
                        color = SKColor.Parse(hex);
                    }
                    bitmap.SetPixel(x, y, color);
                }
            }

            string bitmapFilename = fileName.Replace(".h5", "_image.png");
            using SKImage image = SKImage.FromBitmap(bitmap);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            CacheImageBytes(encoded.ToArray());
            using FileStream stream = System.IO.File.OpenWrite(bitmapFilename);
            encoded.SaveTo(stream);
            _logger.LogInformation($"{DateTime.Now} Saved bitmap to:{bitmapFilename}");
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
            return $"{path}{Path.DirectorySeparatorChar}";
        }

        private void CacheImageBytes(byte[] imageData, string? fileName = "")
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "latestImage";

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ImageCacheService _bitmapCache =
                    scope.ServiceProvider.GetRequiredService<ImageCacheService>();
                _bitmapCache.SetImage(fileName, imageData);
            }
            _logger.LogInformation($" {DateTime.Now} Cached imageData with key::{fileName}");
        }
    }
}