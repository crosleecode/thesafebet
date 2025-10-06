using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace SafeBet.Controllers;

public class RouletteController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public JsonResult Spin()
    {
        // American roulette has 38 numbers: 0, 00, 1-36
        int result = RandomNumberGenerator.GetInt32(0, 38);
        string number;
        string color;
        
        if (result == 0)
        {
            number = "0";
            color = "green";
        }
        else if (result == 1)
        {
            number = "00";
            color = "green";
        }
        else
        {
            number = result.ToString();
            color = new[]{1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36}.Contains(result)
                    ? "red" : "black";
        }

        return Json(new { number = number, color = color });
    }
}
