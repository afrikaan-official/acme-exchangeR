using System.Net.Http;
using System.Threading.Tasks;
using AcmeExchangeR.Utils.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AcmeExchangeR.Utils.FastForexClient
{
    public class FastForexClient : IFastForexClient
    {
        private readonly IOptions<FastForexConfiguration> _options;
        private readonly ILogger<FastForexClient> _logger;
        private readonly HttpClient _httpClient;

        public FastForexClient(IHttpClientFactory httpClientFactory,IOptions<FastForexConfiguration> options,ILogger<FastForexClient> logger)
        {
            _options = options;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("fastForex");
        }
        
        public async Task FetchAll()
        {
            _logger.LogInformation("Fetching all");
            var response = await _httpClient.GetAsync($"/fetch-all?from=USD&api_key={_options.Value.Key}");
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
        }
    }
}