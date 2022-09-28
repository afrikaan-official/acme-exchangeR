using System.ComponentModel.DataAnnotations.Schema;

namespace AcmeExchangeR.Data.Entities;

[Table("ClientLimits")]
public class ClientLimit
{
    public int Id { get; set; }
    public string ClientId { get; set; }
    public DateTime LastTradeDate { get; set; }
    public int Count { get; set; }
}