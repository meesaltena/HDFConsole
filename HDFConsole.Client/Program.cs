using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace HDFConsole.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            string? baseAddres = builder.Configuration.GetValue<string>("BaseAddress");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddres!)});
            await builder.Build().RunAsync();
        }
    }
}
