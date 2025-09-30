namespace SafeBet.Models;

public class Hand
{
    public List<Card> Cards { get; } = new();
    public void Add(Card c) => Cards.Add(c);

    public int Total()
    {
        int sum = Cards.Sum(c => c.Value);
        int aces = Cards.Count(c => c.Rank == Rank.Ace);
        while (sum > 21 && aces-- > 0) sum -= 10; // Ace 11->1
        return sum;
    }
    public bool IsBlackjack() => Cards.Count == 2 && Total() == 21;
    public bool IsBust() => Total() > 21;
}
