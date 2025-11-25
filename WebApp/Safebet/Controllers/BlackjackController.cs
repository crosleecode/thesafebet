using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;
using SafeBet.Services;
using System.Threading.Tasks;

namespace SafeBet.Controllers
{
    public class BlackjackController : Controller
    {
        private static readonly BlackjackGame _game = new();
        private static string? _advicePlaceholder;
        private static int? _bet;
        private readonly SafeAdvisorService _safeAdvisor;
        private readonly SafeBet.Data.SafeBetContext _db;
        const string UserCookie = "sb_anon_id";
        private static bool _advisorUsed = false;

        public BlackjackController(SafeAdvisorService safeAdvisor, SafeBet.Data.SafeBetContext db)
        {
            _safeAdvisor = safeAdvisor;
            _db = db;
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
        public async Task<IActionResult> NewGame()
        {
            _advicePlaceholder = null;
            _bet = 0;
            _advisorUsed = false;
            _game.StartRound();


            if (_game.RoundOver) 
            {
                var id = GetCookieId();
                var m = await GetOrCreateAsync(id);
                var bet = _game.bet ?? 0;

                switch (_game.Result)
                {
                    case RoundResult.PlayerWin:
                        m.BlackjackWins++; m.GamesPlayed++; m.NetEarnings += bet;
                        break;
                    case RoundResult.DealerWin:
                        m.BlackjackLosses++; m.GamesPlayed++; m.NetEarnings -= bet;
                        break;
                    case RoundResult.Push:
                        break;
                }
                m.UpdatedTime = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Hit()
        {
            _advicePlaceholder = null;
            _game.PlayerHit();

            if (_game.RoundOver)
            {
                var id = GetCookieId();
                var m = await GetOrCreateAsync(id);
                var bet = _game.bet ?? 0;

                switch (_game.Result)
                {
                    case RoundResult.PlayerWin:
                        m.BlackjackWins++; m.GamesPlayed++; m.NetEarnings += bet;
                        if (_advisorUsed) m.AdvisedWins++;
                        break;
                    case RoundResult.DealerWin:
                        m.BlackjackLosses++; m.GamesPlayed++; m.NetEarnings -= bet;
                        if (_advisorUsed) m.AdvisedLosses++;
                        break;
                    case RoundResult.Push:
                        break;
                }
                m.UpdatedTime = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                _advisorUsed = false;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Stand()
        {
            _advicePlaceholder = null;
            _game.PlayerStand();

            var id = GetCookieId();
            var m = await GetOrCreateAsync(id);
            var bet = _game.bet ?? 0;

            switch (_game.Result)
            {
                case RoundResult.PlayerWin:
                    m.BlackjackWins++; m.GamesPlayed++; m.NetEarnings += bet;
                    if (_advisorUsed) m.AdvisedWins++;
                    break;
                case RoundResult.DealerWin:
                    m.BlackjackLosses++; m.GamesPlayed++; m.NetEarnings -= bet;
                    if (_advisorUsed) m.AdvisedLosses++;
                    break;
                case RoundResult.Push:
                    break;
            }
            m.UpdatedTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            _advisorUsed = false;

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

            var id = GetCookieId();
            var m = await GetOrCreateAsync(id);

            if (!_advisorUsed)
            {
                m.AdviceRequests++;
            }

            _advisorUsed = true;

            m.UpdatedTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        async Task<SafeBet.Models.Metrics> GetOrCreateAsync(string cookieId)
        {
            var row = await _db.Metrics.FindAsync(cookieId);
            if(row == null)
            {
                row = new SafeBet.Models.Metrics { CookieId = cookieId, UpdatedTime = DateTime.UtcNow };
                _db.Metrics.Add(row);
            }
            return row;
        }

        public string GetCookieId()
        {
            if(!Request.Cookies.TryGetValue(UserCookie, out var id) || string.IsNullOrWhiteSpace(id)){
                id = Guid.NewGuid().ToString("N");
                Response.Cookies.Append(UserCookie, id, new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true
                });
            }
            return id;
        }

    }
}