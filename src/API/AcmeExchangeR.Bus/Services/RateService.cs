using AcmeExchangeR.Data;
using AcmeExchangeR.Data.Entities;
using AcmeExchangeR.Utils.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace AcmeExchangeR.Bus.Services;

public class RateService : IRateService
{
    private readonly ExchangeRateDbContext _dbContext;
    
    public RateService(ExchangeRateDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task SaveAsync(FastForexResponse fastForexResponse, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!cancellationToken.IsCancellationRequested && fastForexResponse != null)
        {
            await _dbContext.ExchangeRates.AddAsync(new ExchangeRate
            {
                Payload = fastForexResponse
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public async Task<ExchangeRate> GetByCurrencyAsync(string currency, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return await _dbContext.ExchangeRates.Where(x => x.Payload.Base == currency)
            .OrderByDescending(x => x.Payload.Updated)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(decimal, string)> TradeAsync(string from,string to,decimal amount,string clientId,CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        //get requested base exchange rate from db
        var dbEntry = await _dbContext.ExchangeRates.Where(x => x.Payload.Base == from)
            .OrderByDescending(x => x.Payload.Updated)
            .FirstOrDefaultAsync(cancellationToken);

        if (dbEntry == null)
        {
            return (0, $"Requested exchange {from} is not in database.");
        }

        // check if to exchange is inside list
        if (!dbEntry.Payload.Results.TryGetProperty(to, out var toCurrency))
        {
            return (0, $"Requested exchange {to} is not in database.");
        }

        var exchangeRate = toCurrency.GetDecimal();
        var result = amount * exchangeRate;

        await _dbContext.TradeHistories.AddAsync(new TradeHistory
        {
            Amount = amount,
            ClientId = clientId,
            From = from,
            To = to,
            Result = result,
            CreatedDate = DateTime.UtcNow
        }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return (result, "");
    }
}