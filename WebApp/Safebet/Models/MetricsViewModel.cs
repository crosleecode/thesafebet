namespace SafeBet.Models;

public class MetricsViewModel
{
    public int GamesPlayed { get; set; } = 0;
    public int AdviceRequests { get; set; } = 0;
    public int BlackjackWins { get; set; } = 0;
    public int BlackjackLosses { get; set; } = 0;

    public string WinLossRatioOverall
    {
        get
        {
            if (BlackjackLosses == 0)
            {
                return "—";
            }
            else
            {
                return $"{(double)BlackjackWins / BlackjackLosses:0.00}";
            }
        }
    }

    public int AdvisedWins { get; set; } = 0;
    public int AdvisedLosses { get; set; } = 0;
    public string WinLossRatioWithAdvice
    {
        get
        {
            if (AdvisedLosses == 0)
            {
                return "—";
            }
            else
            {
                return $"{(double)AdvisedWins / AdvisedLosses:0.00}";
            }
        }
    }

    public decimal NetEarnings { get; set; } = 0.0m;

    public string NetEarningsDisplay
    {
        get
        {
            return NetEarnings.ToString("$#,0.00;-$#,0.00;$0.00");
        }
    }
}
