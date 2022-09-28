using AcmeExchangeR.Bus.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcmeExchangeR.API.Controllers;

[Route("rates")]
public class ExchangeRatesController : ControllerBase
{
    private readonly IRateService _exchangeService;
    
    public ExchangeRatesController(IRateService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    public async Task<IActionResult> Get([FromQuery] string currency, CancellationToken cancellationToken)
    {
        var rate = await _exchangeService.GetByCurrencyAsync(currency, cancellationToken);

        if (rate == null)
        {
            return NotFound("Requested exchange rate is not in database!");
        }

        return Ok(rate);
    }
}