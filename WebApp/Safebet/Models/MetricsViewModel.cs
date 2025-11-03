namespace SafeBet.Models;

public class MetricsViewModel
{
    public int GamesPlayed { get; set; } = 0;
    public int AdviceRequests { get; set; } = 0;
    public int BlackjackWins { get; set; } = 0;
    public int BlackjackLosses { get; set; } = 0;

    public string WinLossRatioOverall =>
        BlackjackLosses == 0 ? "—" : $"{(double)BlackjackWins / BlackjackLosses:0.00}";

    public int AdviceWins { get; set; } = 0;
    public int AdviceLosses { get; set; } = 0;
    public string WinLossRatioWithAdvice =>
        AdviceLosses == 0 ? "—" : $"{(double)AdviceWins / AdviceLosses:0.00}";

    public decimal NetEarnings { get; set; } = 0m;
}
