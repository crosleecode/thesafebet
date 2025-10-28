namespace SafeBet.Models;

public record AdviceRequest(int playerHand, int usableAce, int dealerHand);

public class AdviceRequester
{
    public string advice { get; set; } = "";
    public Dictionary<string, double>? q { get; set; }
    public int[]? state { get; set; }
    public string? source { get; set; }
}