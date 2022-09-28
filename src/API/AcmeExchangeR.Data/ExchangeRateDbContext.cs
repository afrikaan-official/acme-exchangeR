using AcmeExchangeR.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AcmeExchangeR.Data;

public class ExchangeRateDbContext : DbContext
{
    public ExchangeRateDbContext(DbContextOptions<ExchangeRateDbContext> options) : base(options)
    {
        
    }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }
    public DbSet<TradeHistory> TradeHistories { get; set; }
    public DbSet<ClientLimit> ClientLimits { get; set; }
}