using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;

namespace HDFConsole
{
    internal class Program
    {
        private static string baseUrl = "https://api.dataplatform.knmi.nl/open-data/v1/datasets/radar_forecast/versions/1.0/files";


        private static string? resultString = "";

        protected static async Task GetHdf()
        {
            // var result = await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
            HttpClient client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue(apiKey);

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            resultString = response.StatusCode.ToString();

            // return JsonConvert.DeserializeObject<StatusResponse[]>(content);
        }

        static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("Getting HDF");
            await GetHdf();
            await Console.Out.WriteLineAsync(resultString);
        }
    }
}
