using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;
using SafeBet.Services;

namespace SafeBet.Controllers;

public class BlackjackController : Controller
{
    private static readonly BlackjackGame _game = new();
    private static int? _betPlaceholder;      
    private static string? _advicePlaceholder;
    private readonly SafeAdvisorService _safeAdvisor;

    public BlackjackController(SafeAdvisorService safeAdvisor)
    {
        _safeAdvisor = safeAdvisor;
    }

    public IActionResult Index()
    {
        if (_game.Player.Cards.Count == 0 && _game.Result == RoundResult.InProgress)
            _game.StartRound();

        ViewBag.Bet = _betPlaceholder;
        ViewBag.Advice = _advicePlaceholder; // 
        return View(_game);
    }

    [HttpPost]
    public IActionResult NewGame()
    {
        _advicePlaceholder = null;
        _game.StartRound();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Hit()
    {
        _advicePlaceholder = null;
        _game.PlayerHit();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Stand()
    {
        _advicePlaceholder = null;
        _game.PlayerStand();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult SetBet(int? amount)
    {
        _betPlaceholder = (amount is > 0) ? amount : null;
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    public async Task<IActionResult> AskAdvice()
    {
        var req = new AdviceRequest
        {
            PlayerTotal = _game.Player.Total(),
            DealerUpcard = _game.Dealer.Cards.First().Value,
            UsableAce = _game.Player.Cards.Any(c => c.Rank == Rank.Ace) ? 1 : 0
        };

        var result = await _safeAdvisor.GetAdviceAsync(req);
        _advicePlaceholder = result?.advice ?? "No advice";

        return RedirectToAction(nameof(Index));
    }

    /*[HttpPost]
    public IActionResult AskAdvice()
    {
        _advicePlaceholder = null; 
        return RedirectToAction(nameof(Index));
    }*/
}
