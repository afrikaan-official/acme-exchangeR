using AcmeExchangeR.Bus.Services;
using AcmeExchangeR.Utils.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AcmeExchangeR.API.Controllers;

[Route("trade")]
public class TradeController : ControllerBase
{
    private readonly IRateService _exchangeService;

    public TradeController(IRateService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    [HttpPost]
    public async Task<IActionResult> Trade([FromBody] TradeRequest request, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var clientId = Request.Headers["X-Client-Id"].ToString();
            if (string.IsNullOrEmpty(clientId))
            {
                return BadRequest(new { error = $"X-Client-Id header is missing" });
            }

            var (result, error) = await _exchangeService.TradeAsync(request.From, request.To, request.Amount, clientId,
                cancellationToken);
            
            if (!string.IsNullOrEmpty(error))
            {
                return NotFound(new { error });
            }

            return Ok(new { result });
        }

        var errorMessage = string.Join(",/r", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));

        return BadRequest(new { error = errorMessage });
    }
}