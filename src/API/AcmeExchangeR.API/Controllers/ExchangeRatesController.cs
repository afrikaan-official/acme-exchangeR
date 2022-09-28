using AcmeExchangeR.Bus.Services.Abstraction;
using AcmeExchangeR.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AcmeExchangeR.API.Controllers;

[Route("rates")]
public class ExchangeRatesController : ControllerBase
{
    private readonly IRateService _exchangeService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ExchangeRatesController> _logger;
    private readonly IConfiguration _configuration;

    public ExchangeRatesController(IRateService exchangeService,
        IMemoryCache memoryCache,
        ILogger<ExchangeRatesController> logger, 
        IConfiguration configuration)
    {
        _exchangeService = exchangeService;
        _memoryCache = memoryCache;
        _logger = logger;
        _configuration = configuration;
    }
    
    //This endpoint uses in memory cache
    //Cachetimeout can be increased or decreased from appsettings.json
    public async Task<IActionResult> Get([FromQuery] string currency, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Get rates called with currency:{currency}");
        const string key = "acmeExchanger";
        
        if (_memoryCache.TryGetValue(key, out ExchangeRate rateInMemory))
        {
            _logger.LogInformation("Cache hits");
            return Ok(rateInMemory);
        }
        var rate = await _exchangeService.GetByCurrencyAsync(currency, cancellationToken);

        if (rate == null)
        {
            _logger.LogWarning($"{currency} is not in the database!.Maybe update appsetting.json");

            return NotFound("Requested exchange rate is not in database!");
        }
        
        _memoryCache.Set(key, rate, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(_configuration.GetValue<int>("CacheTimeOut")),
            Priority = CacheItemPriority.Normal
        });

        return Ok(rate);
    }
}