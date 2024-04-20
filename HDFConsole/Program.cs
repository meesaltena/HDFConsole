using HDFConsole.Models;
using HDFConsole.Services;

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
            builder.Services.AddOptions<OpenDataClientOptions>()
                .Bind(builder.Configuration.GetSection("OpenDataClientOptions"));
            builder.Services.AddHttpClient<OpenDataService>();
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ImageCacheService>();
            builder.Services.AddScoped<OpenDataService>();
            builder.Services.AddScoped<OpenDataClient>();
            builder.Services.AddHostedService<PeriodicFetcher>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        //builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                        //builder.WithOrigins("http://localhost:4200")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
            var app = builder.Build();

            //app.UseHttpsRedirection();
            app.UseCors("AllowLocalhost");
            app.UseStaticFiles();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
