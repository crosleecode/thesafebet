using System;
using System.Linq;
using System.Collections.Generic;

namespace SafeBet.Models
{
    public enum RoundResult { InProgress, PlayerWin, DealerWin, Push }
    public class BlackjackGame
    {
        public int roundNum { get; private set; } = 0;
        public int? bet { get; private set; } = null;

        public Deck Deck { get; private set; } = new Deck();
        public Hand Player { get; } = new Hand();
        public Hand Dealer { get; } = new Hand();

        public RoundResult Result { get; private set; } = RoundResult.InProgress;
        public bool RoundOver => Result != RoundResult.InProgress;

        public void SetBet(int? amount)
        {
            if (amount.HasValue && amount.Value > 0)
                bet = amount.Value;
            else
                bet = null;
        }
        public void StartRound()
        {
            Deck = new Deck();
            Player.Cards.Clear();
            Dealer.Cards.Clear();

            Result = RoundResult.InProgress;

            Player.Add(Deck.Draw());
            Dealer.Add(Deck.Draw());
            Player.Add(Deck.Draw());
            Dealer.Add(Deck.Draw());

            roundNum++;

            if (Player.IsBlackjack() && Dealer.IsBlackjack())
            {
                Result = RoundResult.Push;
            }
            else if (Player.IsBlackjack())
            {
                Result = RoundResult.PlayerWin;
            }
        }
        public void PlayerHit()
        {
            if (RoundOver) return;

            Player.Add(Deck.Draw());

            if (Player.IsBust())
            {
                Result = RoundResult.DealerWin;
            }
        }

        public void PlayerStand()
        {
            if (RoundOver) return;

            while (Dealer.Total() < 17)
            {
                Dealer.Add(Deck.Draw());
            }

            if (Dealer.IsBust())
            {
                Result = RoundResult.PlayerWin;
                return;
            }

            int p = Player.Total();
            int d = Dealer.Total();

            if (p > d) Result = RoundResult.PlayerWin;
            else if (p < d) Result = RoundResult.DealerWin;
            else Result = RoundResult.Push;
        }
    }
}