using System;

namespace SafeBet.Models
{
    public class StatisticsViewModel
    {
        public int TotalGames { get; set; }
        public int BlackjackGames { get; set; }
        public int RouletteGames { get; set; }
        public int TotalBets { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}