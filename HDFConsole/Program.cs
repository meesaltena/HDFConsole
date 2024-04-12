﻿using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace HDFConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var openDataService = host.Services.GetRequiredService<OpenDataService>();

            var response = await openDataService.GetRecentFiles();
            var file = response?.Files.FirstOrDefault();

            if(file != null)
            {
                string? fileContent = await openDataService.DownloadFile(file.Filename);

                await Console.Out.WriteLineAsync($"Downloaded file with size {fileContent?.Length}");
            }
         
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
                    services.AddHttpClient<OpenDataService>(
                     client =>
                     {
                         client.BaseAddress = new Uri(config["baseAddress"]
                             ?? throw new ArgumentNullException("baseAddress null"));
                         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(config["apiKey"]
                             ?? throw new ArgumentNullException("apiKey null"));
                     });
                });
        }
    }
}
