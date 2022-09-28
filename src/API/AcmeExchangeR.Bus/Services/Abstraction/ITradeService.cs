namespace AcmeExchangeR.Bus.Services.Abstraction;

public interface ITradeService
{
    Task<(decimal, string)> TradeAsync(string from, string to, decimal amount, string clientId,
        CancellationToken cancellationToken);
}