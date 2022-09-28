using AcmeExchangeR.Data.Entities;
using AcmeExchangeR.Utils.Models.Requests;
using AcmeExchangeR.Utils.Models.Responses;

namespace AcmeExchangeR.Bus.Services.Abstraction;

public interface IRateService
{
    Task SaveAsync(FastForexResponse fastForexResponse, CancellationToken cancellationToken);
    Task<ExchangeRate> GetByCurrencyAsync(string currency, CancellationToken cancellationToken);
    
}