using AcmeExchangeR.Data.Entities;
using AcmeExchangeR.Utils.Models.Requests;
using AcmeExchangeR.Utils.Models.Responses;

namespace AcmeExchangeR.Bus.Services;

public interface IRateService
{
    Task SaveAsync(FastForexResponse fastForexResponse, CancellationToken cancellationToken);
    Task<ExchangeRate> GetByCurrencyAsync(string currency, CancellationToken cancellationToken);

    Task<(decimal, string)> TradeAsync(string from, string to, decimal amount, string clientId,
        CancellationToken cancellationToken);
}