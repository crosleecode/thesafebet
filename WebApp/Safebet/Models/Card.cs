namespace SafeBet.Models;

public enum Suit { Clubs, Diamonds, Hearts, Spades }
public enum Rank { Ace=1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

public record Card(Suit Suit, Rank Rank)
{
    public int Value => Rank switch
    {
        Rank.Ace => 11,
        Rank.Jack or Rank.Queen or Rank.King => 10,
        _ => (int)Rank
    };
    public override string ToString() => $"{Rank} of {Suit}";
}
