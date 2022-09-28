using AcmeExchangeR.Bus.Services.Abstraction;
using AcmeExchangeR.Utils.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AcmeExchangeR.API.Controllers;

[Route("trade")]
public class TradeController : ControllerBase
{
    private readonly ITradeService _tradeService;
    private readonly ILogger<TradeController> _logger;

    public TradeController(ITradeService tradeService,ILogger<TradeController> logger)
    {
        _tradeService = tradeService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Trade([FromBody] TradeRequest request, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var clientId = Request.Headers["X-Client-Id"].ToString();
            if (string.IsNullOrEmpty(clientId))
            {
                _logger.LogWarning($"{clientId} header is empty!");

                return BadRequest(new { error = $"X-Client-Id header is missing" });
            }

            var (result, error) = await _tradeService.TradeAsync(request.From, request.To, request.Amount, clientId,
                cancellationToken);
            
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError($"{error}");

                return NotFound(new { error });
            }

            return Ok(new { result });
        }

        var errorMessage = string.Join(",/r", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
        
        _logger.LogError($"Model state is not valid!. Errors: {errorMessage}");


        return BadRequest(new { error = errorMessage });
    }
}