using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Services.IService;
using DrinkCatalog.Utility;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IShoppingCartService _shoppingCartService;
        public HomeController(ICatalogService catalogService, IShoppingCartService shoppingCartService)
        {
            _catalogService = catalogService;
            _shoppingCartService = shoppingCartService;

        }

        public IActionResult Index(string brand, decimal? minPrice, decimal? maxPrice, string sortBy = "Price", int page = 1, int pageSize = 12)
        {
            var drinkCatalogVM = _catalogService.GetDrinkCatalogViewModel(brand, minPrice, maxPrice, sortBy, page, pageSize);

            // Получение количества товаров в корзине из сессии
            var cartItemCount = HttpContext.Session.GetInt32(StaticDetails.SessionCart) ?? 0;
            drinkCatalogVM.CartItemCount = cartItemCount;

            return View(drinkCatalogVM);
        }

        [HttpPost]
        public IActionResult AddCart([FromBody] ShoppingCart shoppingCart)
        {
            var (success, message, cartItemCount) = _shoppingCartService.AddToCartAndUpdateSession(shoppingCart, HttpContext.Session);
            return Json(new { success, message, cartItemCount });
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}