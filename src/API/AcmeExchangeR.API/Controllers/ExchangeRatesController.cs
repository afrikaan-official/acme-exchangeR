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
    
    public ExchangeRatesController(IRateService exchangeService, IMemoryCache memoryCache)
    {
        _exchangeService = exchangeService;
        _memoryCache = memoryCache;
    }
    
    public async Task<IActionResult> Get([FromQuery] string currency, CancellationToken cancellationToken)
    {
        const string key = "acmeExchanger";
        
        if (_memoryCache.TryGetValue(key, out ExchangeRate rateInMemory))
        {
            return Ok(rateInMemory);
        }
        var rate = await _exchangeService.GetByCurrencyAsync(currency, cancellationToken);

        if (rate == null)
        {
            return NotFound("Requested exchange rate is not in database!");
        }
        
        _memoryCache.Set(key, rate, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(20),
            Priority = CacheItemPriority.Normal
        });

        return Ok(rate);
    }
}