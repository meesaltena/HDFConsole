using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace HDFConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }
        
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context,loggingBuilder) => {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));

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
                    services.AddHostedService<PeriodicFetcher>();
                });
        }
    }
}
