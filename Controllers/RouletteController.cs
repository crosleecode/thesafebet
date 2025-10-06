using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;
using System.Security.Cryptography;

namespace SafeBet.Controllers;

public class RouletteController : Controller
{
    private static readonly RouletteGame _game = new();

    public IActionResult Index()
    {
        var viewModel = new RouletteViewModel
        {
            Game = _game,
            BetAmount = 10
        };
        return View(viewModel);
    }

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

    [HttpPost]
    public IActionResult Spin()
    {
        if (_game.CurrentBets.Count == 0)
        {
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

        // Set result message
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

    [HttpPost]
    public IActionResult ClearBets()
    {
        _game.ClearBets();
        TempData["Message"] = "All bets cleared.";
        TempData["MessageType"] = "info";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult ToggleOdds()
    {
        _game.ShowOdds = !_game.ShowOdds;
        return RedirectToAction(nameof(Index));
    }
}
