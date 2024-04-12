using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace HDFConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var openDataClient = host.Services.GetRequiredService<OpenDataClient>();
            await openDataClient.DownloadMostRecentFile(OpenDataDataSets.Actuele10mindataKNMIstations);
            await openDataClient.DownloadMostRecentFile(OpenDataDataSets.radar_forecast);
        }
        
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("appsettings.json");
                })
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    services.AddHttpClient<OpenDataService>("AuthorizedClient",
                     client =>
                     {
                         client.BaseAddress = new Uri(config["baseAddress"]
                             ?? throw new ArgumentNullException("baseAddress null"));
                         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(config["apiKey"]
                             ?? throw new ArgumentNullException("apiKey null"));
                     });

                    services.AddHttpClient<OpenDataService>("AnonymousClient",
                     client =>
                     {
                         client.BaseAddress = new Uri(config["baseAddress"]
                             ?? throw new ArgumentNullException("baseAddress null"));
                     });
                    services.AddScoped<OpenDataService>();
                    services.AddScoped<OpenDataClient>();
                });
        }
    }
}
