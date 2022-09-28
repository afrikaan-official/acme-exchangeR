using System.ComponentModel.DataAnnotations.Schema;

namespace AcmeExchangeR.Data.Entities;

[Table("TradeHistories")]
public class TradeHistory
{
    public int Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public decimal Amount { get; set; }
    public string ClientId { get; set; }
    public decimal Result { get; set; }

    public DateTime CreatedDate { get; set; }
}