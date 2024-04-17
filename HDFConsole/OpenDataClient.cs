using PureHDF;
using SkiaSharp;

namespace HDFConsole
{
    public sealed class OpenDataClient
    {
        private readonly OpenDataService _openDataService = null!;
        private readonly ILogger<OpenDataClient> _logger = null!;
        public OpenDataClient(OpenDataService openDataService, ILogger<OpenDataClient> logger)
        {
            _openDataService = openDataService;
            _logger = logger;
        }


        public async Task DownloadMostRecentFile(OpenDataDataSets dataset, CancellationToken cancellationToken = default)
        {
            var response = await _openDataService.GetRecentFiles(dataset, cancellationToken);
            var file = response?.Files.OrderByDescending(r => r.LastModified).FirstOrDefault();

            if (file == null)
            {
                _logger.LogWarning($"Got file null response from OpenDataService");
                return;
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            string fullPath = @$"{currentDirectory}{Path.DirectorySeparatorChar}{file.Filename}";
            try
            {
                var stream = await _openDataService.DownloadFile(dataset, file.Filename);
                if (stream != null)
                {
                    //TODO add cancellation token
                    using FileStream fileStream = new(fullPath, FileMode.Create);
                    await stream.CopyToAsync(fileStream);
                    _logger.LogInformation($"Wrote {file.Filename} to file {fullPath}");
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
            ReadH5FileAndSaveAsBitMap(fullPath);
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

            using SKBitmap bitmap = new(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte colorIndex = imageData[y, x];
                    SKColor color = infernoPalette[colorIndex % infernoPalette.Length];
                    bitmap.SetPixel(x, y, color);
                }
            }

            string bitmapFilename = fileName.Replace(".h5", "_image.png");
            using SKImage image = SKImage.FromBitmap(bitmap);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            using FileStream stream = System.IO.File.OpenWrite(bitmapFilename);
            encoded.SaveTo(stream);


            _logger.LogInformation($"Saved bitmap to:{bitmapFilename}");
        }
    }
}