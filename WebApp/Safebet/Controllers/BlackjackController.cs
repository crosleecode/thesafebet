using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;
using SafeBet.Services;

namespace SafeBet.Controllers
{
    public class BlackjackController : Controller
    {
        private static readonly BlackjackGame _game = new();
        private static string? _advicePlaceholder;
        private static int? _bet;
        private readonly SafeAdvisorService _safeAdvisor;

        public BlackjackController(SafeAdvisorService safeAdvisor)
        {
            _safeAdvisor = safeAdvisor;
        }
        public IActionResult Index()
        {
         if (_game.Player.Cards.Count == 0 && _game.Result == RoundResult.InProgress)
            {
                _game.StartRound();
            }

            if (_bet.HasValue)
             _game.SetBet(_bet);
            else
             _game.SetBet(null);

            var vm = new BlackjackViewModel
            {
                Game = _game,
                Advice = _advicePlaceholder
            };

            return View(vm);
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
            if (amount.HasValue && amount.Value > 0)
            {
                _bet = amount.Value;
                _game.SetBet(amount.Value);
            }
            else
            {
                _bet = null;
                _game.SetBet(null);
            }

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
    }
}
