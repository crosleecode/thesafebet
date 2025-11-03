using System;

namespace SafeBet.Models
{
    public class BlackjackViewModel
    {
        public BlackjackGame Game { get; set; } = new BlackjackGame();
        public int roundNum => Game.roundNum;
        public int? bet => Game.bet;
        public string? Advice { get; set; }

        public string BetDisplay
        {
            get
            {
                if (bet.HasValue)
                {
                return "$" + bet.Value;
                }
                return "(no bet)";
            }
        }
        public string ResultText
        {
            get
            {
            switch (Game.Result)
                {
                    case RoundResult.PlayerWin: return "You win!";
                    case RoundResult.DealerWin: return "Dealer wins.";
                    case RoundResult.Push:      return "Push or tie.";
                 default: return "";
                }
            }
        }
    }
}
