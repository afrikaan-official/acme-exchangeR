using System.ComponentModel.DataAnnotations.Schema;
using AcmeExchangeR.Utils.Models.Responses;

namespace AcmeExchangeR.Data.Entities;

[Table("ExchangeRates")]
public class ExchangeRate
{
    public int Id { get; set; }
    [Column(TypeName = "jsonb")] 
    public FastForexResponse Payload { get; set; }
}