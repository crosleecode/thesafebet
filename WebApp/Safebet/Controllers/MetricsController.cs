using Microsoft.AspNetCore.Mvc;
using SafeBet.Models;
using SafeBet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SafeBet.Controllers;

[Authorize]
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

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            return Challenge();
        }

        var m = await _db.UserMetrics.FindAsync(userId);

        if (m == null)
        {
            return View(new MetricsViewModel());
        }

        var vm = new MetricsViewModel
        {
            GamesPlayed = m.GamesPlayed,
            AdviceRequests = m.AdviceRequests,
            BlackjackWins = m.BlackjackWins,
            BlackjackLosses = m.BlackjackLosses,
            AdvisedWins = m.AdvisedWins,
            AdvisedLosses = m.AdvisedLosses,
            NetEarnings = m.NetEarnings
        };

        return View(vm);
    }
}