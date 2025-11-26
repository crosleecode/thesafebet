using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;

namespace SafeBet.Controllers
{
    public class StatisticsController : Controller
    {
        public IActionResult Index()
        {
            var model = new StatisticsViewModel
            {
                TotalGames = 120,
                BlackjackGames = 70,
                RouletteGames = 50,
                TotalBets = 5600,
                TotalWins = 45,
                TotalLosses = 75,
                LastUpdated = DateTime.Now
            };

            return View(model);
        }
    }
}