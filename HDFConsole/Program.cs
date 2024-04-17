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
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context,loggingBuilder) => {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));

                   })
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions<OpenDataServiceOptions>().Bind(context.Configuration.GetSection("OpenDataServiceOptions"));
                    services.AddHttpClient<OpenDataService>();
                    services.AddScoped<OpenDataService>();
                    services.AddScoped<OpenDataClient>();
                    services.AddHostedService<PeriodicFetcher>();
                });

            return hostBuilder;
        }
    }
}
