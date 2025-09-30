namespace SafeBet.Models;

public enum RoundResult { InProgress, PlayerWin, DealerWin, Push }

public class BlackjackGame
{
    public Deck Deck { get; private set; } = new();
    public Hand Player { get; } = new();
    public Hand Dealer { get; } = new();
    public RoundResult Result { get; private set; } = RoundResult.InProgress;
    public bool RoundOver => Result != RoundResult.InProgress;

    public void StartRound()
    {
        Deck = new Deck();
        Player.Cards.Clear(); Dealer.Cards.Clear();
        Result = RoundResult.InProgress;

        Player.Add(Deck.Draw()); Dealer.Add(Deck.Draw());
        Player.Add(Deck.Draw()); Dealer.Add(Deck.Draw());

        if (Player.IsBlackjack() && Dealer.IsBlackjack()) Result = RoundResult.Push;
        else if (Player.IsBlackjack()) Result = RoundResult.PlayerWin;
    }

    public void PlayerHit()
    {
        if (RoundOver) return;
        Player.Add(Deck.Draw());
        if (Player.IsBust()) Result = RoundResult.DealerWin;
    }

    public void PlayerStand()
    {
        if (RoundOver) return;
        while (Dealer.Total() < 17) Dealer.Add(Deck.Draw());
        if (Dealer.IsBust()) { Result = RoundResult.PlayerWin; return; }

        int p = Player.Total(), d = Dealer.Total();
        Result = p > d ? RoundResult.PlayerWin : p < d ? RoundResult.DealerWin : RoundResult.Push;
    }
}
