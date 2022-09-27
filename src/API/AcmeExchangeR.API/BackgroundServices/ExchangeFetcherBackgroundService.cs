using AcmeExchangeR.Utils.FastForexClient;

namespace AcmeExchangeR.API.BackgroundServices;

// Will run in the background to fetch exchange from resources (fastForex) and store it to database
//Will run in every 15 minutes
public class ExchangeFetcherBackgroundService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(15);
    private readonly ILogger<ExchangeFetcherBackgroundService> _logger;
    private readonly IServiceScopeFactory _factory;

    public ExchangeFetcherBackgroundService(ILogger<ExchangeFetcherBackgroundService> logger, IServiceScopeFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var timer = new PeriodicTimer(_period))
        {
            while (await timer.WaitForNextTickAsync(stoppingToken) &&
                   !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using (var scope = _factory.CreateAsyncScope())
                    {
                        var sampleService = scope.ServiceProvider.GetRequiredService<IFastForexClient>();
                        
                        await sampleService.FetchAll();
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