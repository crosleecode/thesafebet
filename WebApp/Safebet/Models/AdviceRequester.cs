namespace SafeBet.Models;
using System.Text.Json.Serialization;

public sealed class AdviceRequest
{
    [JsonPropertyName("player_total")]
    public int PlayerTotal { get; set; }

    [JsonPropertyName("dealer_upcard")]
    public int DealerUpcard { get; set; }

    [JsonPropertyName("usable_ace")]
    public int UsableAce { get; set; }
}

public class AdviceRequester
{
    public string advice { get; set; } = "";
    public Dictionary<string, double>? q { get; set; } //Not really used
    public int[]? state { get; set; }
    public string? source { get; set; }
}