using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;

namespace SafeBet.Controllers
{
    public class BlackjackController : Controller
    {
        private static BlackjackGame _game = new BlackjackGame();
        private static int? _bet;
        private static string? _advicePlaceholder;

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
                Game = _game
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
        public IActionResult AskAdvice()
        {
            _advicePlaceholder = null;
            return RedirectToAction(nameof(Index));
        }
    }
}
