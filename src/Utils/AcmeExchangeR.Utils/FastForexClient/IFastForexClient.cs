using AcmeExchangeR.Utils.Models;
using AcmeExchangeR.Utils.Models.Responses;

namespace AcmeExchangeR.Utils.FastForexClient
{
    public interface IFastForexClient
    {
        Task<FastForexResponse> FetchExchangeRateAsync(string currency, CancellationToken cancellationToken);
    }
}