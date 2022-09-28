namespace AcmeExchangeR.Utils.Models.Requests;

public class TradeRequest
{
    public string From { get; set; }
    public string To { get; set; }
    public decimal Amount { get; set; }
}