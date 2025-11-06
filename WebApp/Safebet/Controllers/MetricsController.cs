using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;
using SafeBet.Data;
using Microsoft.EntityFrameworkCore;

namespace SafeBet.Controllers;

public class MetricsController : Controller
{
    private readonly SafeBetContext _db;

    public MetricsController(SafeBetContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new MetricsViewModel
        {
            GamesPlayed = await _db.Metrics.SumAsync(m => m.GamesPlayed),
            AdviceRequests = await _db.Metrics.SumAsync(m => m.AdviceRequests),
            BlackjackWins = await _db.Metrics.SumAsync(m => m.BlackjackWins),
            BlackjackLosses = await _db.Metrics.SumAsync(m => m.BlackjackLosses),
            AdvisedWins = await _db.Metrics.SumAsync(m => m.AdvisedWins),
            AdvisedLosses = await _db.Metrics.SumAsync(m => m.AdvisedLosses),
            NetEarnings = await _db.Metrics.SumAsync(m => (decimal)m.NetEarnings)
        };
        return View(vm);
    }
}