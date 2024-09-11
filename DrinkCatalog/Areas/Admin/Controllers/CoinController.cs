using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CoinController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoinController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var coins = _unitOfWork.Coins.GetAll()
                                         .OrderBy(u => u.Denomination)
                                         .ToList();

            return View(coins);
        }

        public IActionResult AddCoin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCoin(Coin coin)
        {
            int[] allowedDenominations = { 1, 2, 5, 10 };
            if (!allowedDenominations.Contains(coin.Denomination))
            {
                ModelState.AddModelError("", "Номинал монеты должен быть 1, 2, 5 или 10 рублей.");
                TempData["ErrorMessage"] = "Номинал монеты должен быть 1, 2, 5 или 10 рублей.";
                return View(coin);
            }

            var existingCoin = _unitOfWork.Coins.GetFirstOrDefault(u => u.Denomination == coin.Denomination);
            if (existingCoin != null)
            {
                existingCoin.Count += coin.Count;
                _unitOfWork.Coins.Update(existingCoin);
                TempData["SuccessMessage"] = "Количество монет успешно обновлено";
            }
            else
            {
                _unitOfWork.Coins.Add(coin);
                TempData["SuccessMessage"] = "Новая монета успешно добавлена";
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditCoin(int id)
        {
            var coin = _unitOfWork.Coins.GetById(u => u.Id == id);
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
                _unitOfWork.Coins.Update(coin);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Монета успешно отредактирована";
                return RedirectToAction(nameof(Index));
            }
            return View(coin);
        }
    }
}
