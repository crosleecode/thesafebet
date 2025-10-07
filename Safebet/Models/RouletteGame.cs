using System.ComponentModel.DataAnnotations;

namespace SafeBet.Models;

public class RouletteGame
{
    public List<Bet> CurrentBets { get; set; } = new();
    public int? LastResult { get; set; }
    public string? LastColor { get; set; }
    public int TotalWinnings { get; set; }
    public List<GameHistory> GameHistory { get; set; } = new();
    public bool ShowOdds { get; set; }
    
    public int GetTotalBetAmount()
    {
        return CurrentBets.Sum(bet => bet.Amount);
    }
    
    public void AddBet(string betType, int amount)
    {
        var existingBet = CurrentBets.FirstOrDefault(b => b.BetType == betType);
        if (existingBet != null)
        {
            existingBet.Amount += amount;
        }
        else
        {
            CurrentBets.Add(new Bet { BetType = betType, Amount = amount });
        }
    }
    
    public void ClearBets()
    {
        CurrentBets.Clear();
    }
    
    public int CalculateWinnings(int result, string color)
    {
        int totalWinnings = 0;
        foreach (var bet in CurrentBets)
        {
            if (IsWinningBet(bet.BetType, result, color))
            {
                totalWinnings += bet.Amount * GetPayout(bet.BetType);
            }
        }
        return totalWinnings;
    }
    
    private bool IsWinningBet(string betType, int result, string color)
    {
        return betType switch
        {
            "0" => result == 0,
            "00" => result == 00,
            "red" => color == "red",
            "black" => color == "black",
            "even" => result > 0 && result % 2 == 0,
            "odd" => result > 0 && result % 2 == 1,
            "1to18" => result >= 1 && result <= 18,
            "19to36" => result >= 19 && result <= 36,
            "1st12" => result >= 1 && result <= 12,
            "2nd12" => result >= 13 && result <= 24,
            "3rd12" => result >= 25 && result <= 36,
            "col1" => new[] { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 }.Contains(result),
            "col2" => new[] { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 }.Contains(result),
            "col3" => new[] { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 }.Contains(result),
            _ => int.TryParse(betType, out int number) && number == result
        };
    }
    
    private int GetPayout(string betType)
    {
        return betType switch
        {
            "red" or "black" or "even" or "odd" or "1to18" or "19to36" => 1,
            "1st12" or "2nd12" or "3rd12" or "col1" or "col2" or "col3" => 2,
            _ => 35 // Individual numbers
        };
    }
}

public class Bet
{
    public string BetType { get; set; } = string.Empty;
    public int Amount { get; set; }
    public int Payout => GetPayout();
    
    private int GetPayout()
    {
        return BetType switch
        {
            "red" or "black" or "even" or "odd" or "1to18" or "19to36" => 1,
            "1st12" or "2nd12" or "3rd12" or "col1" or "col2" or "col3" => 2,
            _ => 35 // Individual numbers
        };
    }
}

public class GameHistory
{
    public int Result { get; set; }
    public string Color { get; set; } = string.Empty;
    public int TotalBet { get; set; }
    public int Winnings { get; set; }
    public int NetWinnings { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class RouletteViewModel
{
    public RouletteGame Game { get; set; } = new();
    public int BetAmount { get; set; } = 10;
    public string? Message { get; set; }
    public string? MessageType { get; set; } // "success", "error", "info"
}
