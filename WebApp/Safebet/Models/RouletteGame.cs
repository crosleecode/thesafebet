using System.ComponentModel.DataAnnotations;

namespace SafeBet.Models;

/// <summary>
/// Main game state management class for Roulette game.
/// Manages bets, game history, and winning calculations.
/// </summary>
public class RouletteGame
{
    // ========== PROPERTIES ==========
    
    /// <summary>List of current active bets placed by the player</summary>
    public List<Bet> CurrentBets { get; set; } = new();
    
    /// <summary>Last winning number result (0-36, or 00)</summary>
    public int? LastResult { get; set; }
    
    /// <summary>Color of the last winning number (red, black, or green)</summary>
    public string? LastColor { get; set; }
    
    /// <summary>Total cumulative winnings across all games</summary>
    public int TotalWinnings { get; set; }
    
    /// <summary>History of all game rounds with results and winnings</summary>
    public List<GameHistory> GameHistory { get; set; } = new();
    
    /// <summary>Flag to show/hide odds information on the betting table</summary>
    public bool ShowOdds { get; set; }
    
    // ========== PUBLIC METHODS ==========
    
    /// <summary>
    /// Calculates the total amount of all current bets
    /// </summary>
    /// <returns>Sum of all bet amounts</returns>
    public int GetTotalBetAmount()
    {
        return CurrentBets.Sum(bet => bet.Amount);
    }
    
    /// <summary>
    /// Adds a new bet or increases amount of existing bet
    /// </summary>
    /// <param name="betType">Type of bet (e.g., "red", "black", "1", "1st12", etc.)</param>
    /// <param name="amount">Bet amount in dollars</param>
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
    
    /// <summary>
    /// Clears all current bets from the game
    /// </summary>
    public void ClearBets()
    {
        CurrentBets.Clear();
    }
    
    /// <summary>
    /// Calculates total winnings based on result and color
    /// </summary>
    /// <param name="result">Winning number (0-36, or 00)</param>
    /// <param name="color">Winning color (red, black, or green)</param>
    /// <returns>Total winnings amount</returns>
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
    
    // ========== PRIVATE METHODS ==========
    
    /// <summary>
    /// Determines if a bet is a winning bet based on result and color
    /// </summary>
    /// <param name="betType">Type of bet to check</param>
    /// <param name="result">Winning number</param>
    /// <param name="color">Winning color</param>
    /// <returns>True if bet wins, false otherwise</returns>
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
    
    /// <summary>
    /// Gets the payout multiplier for a bet type
    /// </summary>
    /// <param name="betType">Type of bet</param>
    /// <returns>Payout multiplier (1:1, 2:1, or 35:1)</returns>
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

/// <summary>
/// Represents a single bet placed by the player
/// </summary>
public class Bet
{
    // ========== PROPERTIES ==========
    
    /// <summary>Type of bet (e.g., "red", "black", "1", "1st12", "col1", etc.)</summary>
    public string BetType { get; set; } = string.Empty;
    
    /// <summary>Amount of money bet</summary>
    public int Amount { get; set; }
    
    /// <summary>Payout multiplier for this bet type (1:1, 2:1, or 35:1)</summary>
    public int Payout => GetPayout();
    
    // ========== PRIVATE METHODS ==========
    
    /// <summary>
    /// Calculates the payout multiplier based on bet type
    /// </summary>
    /// <returns>Payout multiplier</returns>
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

/// <summary>
/// Represents a single game round history record
/// </summary>
public class GameHistory
{
    // ========== PROPERTIES ==========
    
    /// <summary>Winning number result</summary>
    public int Result { get; set; }
    
    /// <summary>Color of winning number (red, black, or green)</summary>
    public string Color { get; set; } = string.Empty;
    
    /// <summary>Total amount bet in this round</summary>
    public int TotalBet { get; set; }
    
    /// <summary>Total winnings from this round</summary>
    public int Winnings { get; set; }
    
    /// <summary>Net winnings (winnings - total bet)</summary>
    public int NetWinnings { get; set; }
    
    /// <summary>Timestamp when the game round occurred</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// View model for Roulette page data binding
/// </summary>
public class RouletteViewModel
{
    // ========== PROPERTIES ==========
    
    /// <summary>Current game state and data</summary>
    public RouletteGame Game { get; set; } = new();
    
    /// <summary>Default bet amount for the bet input field</summary>
    public int BetAmount { get; set; } = 10;
    
    /// <summary>Message to display to the user</summary>
    public string? Message { get; set; }
    
    /// <summary>Type of message: "success", "error", or "info"</summary>
    public string? MessageType { get; set; }
}
