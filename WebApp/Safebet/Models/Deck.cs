namespace SafeBet.Models;

public class Deck
{
    private readonly Stack<Card> _cards = new();
    private static readonly Random Rng = new();

    public Deck()
    {
        var all = new List<Card>();
        foreach (Suit s in Enum.GetValues(typeof(Suit)))
            foreach (Rank r in Enum.GetValues(typeof(Rank)))
                all.Add(new Card(s, r));
        foreach (var c in all.OrderBy(_ => Rng.Next())) _cards.Push(c);
    }
    public Card Draw() => _cards.Pop();
}
