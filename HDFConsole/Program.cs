namespace HDFConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.WebHost.ConfigureKestrel(options => options.)
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
                loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            });
            builder.Services.AddOptions<OpenDataServiceOptions>()
                .Bind(builder.Configuration.GetSection("OpenDataServiceOptions"));
            builder.Services.AddHttpClient<OpenDataService>();
      
            builder.Services.AddScoped<OpenDataService>();
            builder.Services.AddScoped<OpenDataClient>();
            builder.Services.AddHostedService<PeriodicFetcher>();
            builder.Services.AddControllers();

            var app = builder.Build();
           
            //app.UseHttpsRedirection();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
