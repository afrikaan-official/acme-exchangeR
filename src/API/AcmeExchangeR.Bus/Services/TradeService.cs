using AcmeExchangeR.Bus.Services.Abstraction;
using AcmeExchangeR.Data;
using AcmeExchangeR.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AcmeExchangeR.Bus.Services;

public class TradeService : ITradeService
{
    private readonly ExchangeRateDbContext _dbContext;

    public TradeService(ExchangeRateDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<(decimal, string)> TradeAsync(string from,string to,decimal amount,string clientId,CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var utcNow = DateTime.UtcNow;
        
        //Check if client has exceed 1 hour limit (Bonus)
        var clientLimit =
            await _dbContext.ClientLimits.FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);

        // if clientLimit is not null check if client has limit
        if (clientLimit != null)
        {
            var diff = DateTime.UtcNow - clientLimit.LastTradeDate;
            //check if client exceeds 10 trade count
            if (diff.Minutes <= 60)
            {
                if (clientLimit.Count >= 10)
                {
                    return (0, "You cannot trade more then 10 times per hour.");
                }
                //if client has limit increment count
                clientLimit.Count++;
                clientLimit.LastTradeDate = utcNow;
            }
            else if (diff.Minutes >= 60) //reset client limit
            {
                clientLimit.Count = 0;
                _dbContext.Entry(clientLimit).State = EntityState.Modified;
            }
        }

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
            CreatedDate = utcNow
        }, cancellationToken);

        //if clientLimit is null that means client is trading for the first time 
        if (clientLimit == null)
        {
            await _dbContext.ClientLimits.AddAsync(new ClientLimit
            {
                Count = 1,
                ClientId = clientId,
                LastTradeDate = utcNow
            }, cancellationToken);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return (result, "");
    }
}