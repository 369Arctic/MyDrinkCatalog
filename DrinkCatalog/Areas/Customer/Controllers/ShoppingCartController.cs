using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;
using DrinkCatalog.Utility;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        // Отображение корзины
        public IActionResult Index()
        {
            var cartItems = _shoppingCartService.GetAllCartItems().ToList();

            if (!cartItems.Any())
            {
                TempData["Message"] = "У вас нет ни одного товара, вернитесь на страницу каталога.";
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, 0);
            }

            var cartTotal = cartItems.Sum(x => x.Drink.Price * x.Count);
            var cartItemCount = cartItems.Sum(u => u.Count);

            HttpContext.Session.SetInt32(StaticDetails.SessionCart, cartItemCount);

            var shoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartsList = cartItems.OrderBy(u => u.Id),
                CartTotal = cartTotal,
                CartItemCount = cartItemCount,
                Message = TempData["Message"]?.ToString()
            };
            return View(shoppingCartVM);
        }


        // Удаление товара из корзины
        [HttpPost]
        public IActionResult RemoveFromCart(int cartId)
        {
            _shoppingCartService.RemoveFromCart(cartId);
            return RedirectToAction(nameof(Index));
        }

        // Обновление количества товара
        [HttpPost]
        public IActionResult UpdateCart(int cartId, int newCount)
        {
            var (success, errorMessage) = _shoppingCartService.UpdateCartItemCount(cartId, newCount);
            return Json(new { success, errorMessage });
        }

        // Получение кол-ва товара
        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            var cartItemCount = _shoppingCartService.GetCartItemCount();
            return Json(new { cartItemCount });
        }
    }
}

