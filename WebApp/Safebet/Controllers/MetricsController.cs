using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;

namespace SafeBet.Controllers;

public class MetricsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var vm = new MetricsViewModel
        {
            GamesPlayed = 12,
            AdviceRequests = 7,
            BlackjackWins = 5,
            BlackjackLosses = 7,
            AdviceWins = 3,
            AdviceLosses = 4,
            NetEarnings = -37.00m
        };
        return View(vm);
    }
}