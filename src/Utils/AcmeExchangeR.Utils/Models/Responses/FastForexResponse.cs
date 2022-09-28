using System.Text.Json;
using System.Text.Json.Serialization;

namespace AcmeExchangeR.Utils.Models.Responses;

public class FastForexResponse
{
    [JsonPropertyName("base")]
    public string Base { get; set; }
    [JsonPropertyName("results")]
    public JsonElement Results { get; set; }
    [JsonPropertyName("updated")]
    public string Updated { get; set; }
}