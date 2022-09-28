using AcmeExchangeR.Data;
using AcmeExchangeR.Data.Entities;
using AcmeExchangeR.Utils.Models.Responses;
using Microsoft.EntityFrameworkCore;
using AcmeExchangeR.Bus.Services.Abstraction;

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
}