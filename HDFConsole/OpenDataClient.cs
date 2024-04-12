using Microsoft.Extensions.Logging;
using System.IO;
using PureHDF;
using System.Drawing;
using System.Drawing.Imaging;

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


        public async Task DownloadMostRecentFile(OpenDataDataSets dataset)
        {
            var response = await _openDataService.GetRecentFiles(dataset);
            var file = response?.Files.OrderByDescending(r => r.LastModified).FirstOrDefault();

            if (file == null)
            {
                _logger.LogWarning($"Got file null response from OpenDataService");
                return;
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            string fullPath = @$"{currentDirectory}\{file.Filename}";
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

        private readonly Color[] infernoPalette = [
            Color.FromArgb(255, 20, 11, 52),
            Color.FromArgb(255, 132, 32, 107),
            Color.FromArgb(255, 229, 92, 48),
            Color.FromArgb(255, 246, 215, 70)
            ];

        private void ReadH5FileAndSaveAsBitMap(string fileName)
        {
            var hdfFile = H5File.OpenRead(fileName);
            var imageGroup = hdfFile.Group("/image1");
            var imageData = imageGroup.Dataset("image_data").Read<byte[,]>();
            //var vizGroup = hdfFile.Group("/visualisation1");
            //var colorPalette = vizGroup.Dataset("color_palette").Read<byte[,]>(); // 256x3 

#pragma warning disable CA1416
            Bitmap bitmap = new Bitmap(imageData.GetLength(1), imageData.GetLength(0));

            for (int y = 0; y < imageData.GetLength(0); y++)
            {
                for (int x = 0; x < imageData.GetLength(1); x++)
                {
                    //byte colorIndex = imageData[y, x];
                    //var r = colorPalette[colorIndex, 0];
                    //var g = colorPalette[colorIndex, 1];
                    //var b = colorPalette[colorIndex, 2];

                    byte colorIndex = imageData[y, x];
                    // Map the color index to a color in the inferno palette
                    Color color = infernoPalette[colorIndex % infernoPalette.Length];
                    //byte colorValue = (byte)(255 - imageData[y, x]); // Invert color value
                    //Color color = Color.FromArgb(colorValue, colorValue, colorValue);
                    //Color color = Color.FromArgb(75,255-colorPalette[colorIndex, 0],255-colorPalette[colorIndex, 1],255-colorPalette[colorIndex, 2]);
                    bitmap.SetPixel(x, y, color);
                }
            }
            string bitmapFilename = fileName.Replace(".h5", "_image.png");
            _logger.LogInformation($"Saved bitmap to:{bitmapFilename}");
            bitmap.Save(bitmapFilename, ImageFormat.Png);
#pragma warning restore CA1416
        }
    }
}