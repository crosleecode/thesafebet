using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;
using System.Security.Cryptography;

namespace SafeBet.Controllers;

/// <summary>
/// Controller for Roulette game functionality.
/// Handles game state, bet placement, spinning, and game history.
/// </summary>
public class RouletteController : Controller
{
    // ========== FIELDS ==========
    
    /// <summary>Static game instance shared across all requests</summary>
    private static readonly RouletteGame _game = new();

    // ========== ACTION METHODS ==========
    
    /// <summary>
    /// Displays the main Roulette game page
    /// </summary>
    /// <returns>Roulette Index view with game state</returns>
    public IActionResult Index()
    {
        var viewModel = new RouletteViewModel
        {
            Game = _game,
            BetAmount = 10
        };
        return View(viewModel);
    }

    /// <summary>
    /// Handles bet placement from the user
    /// </summary>
    /// <param name="betType">Type of bet to place</param>
    /// <param name="betAmount">Amount to bet</param>
    /// <returns>Redirects to Index or returns error</returns>
    [HttpPost]
    public IActionResult PlaceBet(string betType, int betAmount)
    {
        if (betAmount <= 0)
        {
            TempData["Message"] = "Please enter a valid bet amount.";
            TempData["MessageType"] = "error";
            return RedirectToAction(nameof(Index));
        }

        _game.AddBet(betType, betAmount);
        TempData["Message"] = $"Bet placed: {betType} for ${betAmount}";
        TempData["MessageType"] = "success";
        
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Executes a game spin, generates random result, calculates winnings
    /// </summary>
    /// <returns>JSON response for AJAX or redirect for form submission</returns>
    [HttpPost]
    public IActionResult Spin()
    {
        if (_game.CurrentBets.Count == 0)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Please place at least one bet before spinning." });
            }
            TempData["Message"] = "Please place at least one bet before spinning.";
            TempData["MessageType"] = "error";
            return RedirectToAction(nameof(Index));
        }

        // Generate random result
        int result = RandomNumberGenerator.GetInt32(0, 38);
        string color;
        
        if (result == 0)
        {
            result = 0;
            color = "green";
        }
        else if (result == 1)
        {
            result = 00;
            color = "green";
        }
        else
        {
            color = new[]{1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36}.Contains(result)
                    ? "red" : "black";
        }

        // Calculate winnings
        int winnings = _game.CalculateWinnings(result, color);
        int totalBet = _game.GetTotalBetAmount();
        int netWinnings = winnings - totalBet;
        
        // Update game state
        _game.LastResult = result;
        _game.LastColor = color;
        _game.TotalWinnings += netWinnings;
        
        // Add to history
        _game.GameHistory.Add(new GameHistory
        {
            Result = result,
            Color = color,
            TotalBet = totalBet,
            Winnings = winnings,
            NetWinnings = netWinnings
        });

        // Clear current bets
        _game.ClearBets();

        // If AJAX request, return JSON
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new 
            { 
                success = true, 
                winner = result,
                color = color,
                winnings = winnings,
                netWinnings = netWinnings,
                message = netWinnings > 0 ? $"You won ${netWinnings}! Result: {result} ({color})" :
                         netWinnings < 0 ? $"You lost ${Math.Abs(netWinnings)}. Result: {result} ({color})" :
                         $"You broke even! Result: {result} ({color})"
            });
        }

        // Set result message for regular form submission
        if (netWinnings > 0)
        {
            TempData["Message"] = $"You won ${netWinnings}! Result: {result} ({color})";
            TempData["MessageType"] = "success";
        }
        else if (netWinnings < 0)
        {
            TempData["Message"] = $"You lost ${Math.Abs(netWinnings)}. Result: {result} ({color})";
            TempData["MessageType"] = "error";
        }
        else
        {
            TempData["Message"] = $"You broke even! Result: {result} ({color})";
            TempData["MessageType"] = "info";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Clears all current bets from the game
    /// </summary>
    /// <returns>Redirects to Index</returns>
    [HttpPost]
    public IActionResult ClearBets()
    {
        _game.ClearBets();
        TempData["Message"] = "All bets cleared.";
        TempData["MessageType"] = "info";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Toggles the display of odds on the betting table
    /// </summary>
    /// <returns>Redirects to Index</returns>
    [HttpPost]
    public IActionResult ToggleOdds()
    {
        _game.ShowOdds = !_game.ShowOdds;
        return RedirectToAction(nameof(Index));
    }
}
