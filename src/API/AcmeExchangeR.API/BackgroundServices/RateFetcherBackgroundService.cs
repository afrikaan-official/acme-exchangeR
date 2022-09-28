using AcmeExchangeR.Bus.Services.Abstraction;
using AcmeExchangeR.Utils.FastForexClient;

namespace AcmeExchangeR.API.BackgroundServices;

// Will run in the background (every 10 seconds) to fetch exchange from resources (fastForex) and store it to database
// Interval can be increased or decreased from  appsettings.json
public class RateFetcherBackgroundService : BackgroundService
{
    private readonly ILogger<RateFetcherBackgroundService> _logger;
    private readonly IServiceScopeFactory _factory;
    private readonly IConfiguration _configuration;

    public RateFetcherBackgroundService(ILogger<RateFetcherBackgroundService> logger, IServiceScopeFactory factory, IConfiguration configuration)
    {
        _logger = logger;
        _factory = factory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var period = TimeSpan.FromSeconds(_configuration.GetValue<int>("BackgroundServiceInterval"));

        using (var timer = new PeriodicTimer(period))
        {
            while (await timer.WaitForNextTickAsync(stoppingToken) &&
                   !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using (var scope = _factory.CreateAsyncScope())
                    {
                        var fastForexClient = scope.ServiceProvider.GetRequiredService<IFastForexClient>();
                        var exchangeService = scope.ServiceProvider.GetRequiredService<IRateService>();
                        
                        //get all active exchanges from configuration
                        var exchanges = _configuration.GetSection("Exchanges").GetChildren();
                        //create tasks
                        var fetchTasks = exchanges.Select(x => fastForexClient.FetchExchangeRateAsync(x.Value, stoppingToken)).ToArray();
                        //send multiple requests to fastForex in paralel
                        var fetchResults = await Task.WhenAll(fetchTasks);

                        foreach (var fetchResult in fetchResults)
                        {
                            await exchangeService.SaveAsync(fetchResult, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Error in ExchangeFetcherBackgroundService. Exception: {ex.Message}");
                }
            }
        }
    }
}