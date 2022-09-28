using System.Text.Json;
using AcmeExchangeR.Utils.Models.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AcmeExchangeR.Utils.FastForexClient
{
    public class FastForexClient : IFastForexClient
    {
        private readonly IConfiguration _options;
        private readonly ILogger<FastForexClient> _logger;
        private readonly HttpClient _httpClient;

        public FastForexClient(IHttpClientFactory httpClientFactory,
            IConfiguration options,
            ILogger<FastForexClient> logger)
        {
            _options = options;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("fastForex");
        }

        public async Task<FastForexResponse> FetchExchangeRateAsync(string currency,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var apiKey = _options["FastForexAPI:Key"];
            _logger.LogInformation($"Fetching exchange rates for {currency}");

            var response =
                await _httpClient.GetAsync($"/fetch-all?from={currency}&api_key={apiKey}", cancellationToken);

            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            
            return JsonSerializer.Deserialize<FastForexResponse>(jsonResponse);
        }
    }
}