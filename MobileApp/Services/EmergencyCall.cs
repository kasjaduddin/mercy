using RestSharp;
using System.Diagnostics;

namespace MobileApp.Services
{
    public class EmergencyCall
    {
        private readonly string _apiBaseUrl = "https://pevlgv.api.infobip.com";
        private readonly RestClient _client;
        private readonly string? _apiKey = Environment.GetEnvironmentVariable("API_KEY");
        private readonly string? _sender = Environment.GetEnvironmentVariable("SENDER");
        
        public EmergencyCall()
        {
            var options = new RestClientOptions(_apiBaseUrl)
            {
                Timeout = TimeSpan.FromMilliseconds(-1),
            };
            _client = new RestClient(options);
        }

        public async Task SendEmergencyMessage(string[] destinationNumbers, string text)
        {
            var request = new RestRequest("/tts/3/advanced", Method.Post);
            request.AddHeader("Authorization", $"App {_apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");

            var destinations = destinationNumbers.Select(number => new { to = number }).ToArray();
            var body = new
            {
                messages = new[]
                {
                    new
                    {
                        destinations = destinations,
                        from = _sender,
                        language = "id",
                        text,
                        voice = new
                        {
                            name = "Indah (beta)",
                            gender = "female"
                        }
                    }
                }
            };

            request.AddJsonBody(body);
            var response = await _client.ExecuteAsync(request);
            Console.WriteLine(response.Content);
        }
    }
}
