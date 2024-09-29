using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CoinController : Controller
    {
        private readonly ICoinService _coinService;

        public CoinController(ICoinService coinService)
        {
            _coinService = coinService;
        }

        public IActionResult Index()
        {
            var coins = _coinService.GetAllCoins();
            return View(coins);
        }

        public IActionResult AddCoin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCoin(Coin coin)
        {
            var result = _coinService.AddOrUpdateCoin(coin);

            if (result.Contains("Номинал монеты"))
            {
                ModelState.AddModelError("", result);
                TempData["ErrorMessage"] = result;
                return View(coin);
            }

            TempData["SuccessMessage"] = result;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditCoin(int id)
        {
            var coin = _coinService.GetCoinById(id);
            if (coin == null)
            {
                return NotFound();
            }
            return View(coin);
        }

        [HttpPost]
        public IActionResult EditCoin(Coin coin)
        {
            if (ModelState.IsValid)
            {
                var result = _coinService.EditCoin(coin);
                TempData["SuccessMessage"] = result;
                return RedirectToAction(nameof(Index));
            }
            return View(coin);
        }
    }
}
