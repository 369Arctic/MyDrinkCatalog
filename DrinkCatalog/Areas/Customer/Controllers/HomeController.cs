using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Utility;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartVM ShoppingCartVM { get; set;}

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string brand, decimal? minPrice, decimal? maxPrice, string sortBy = "Price", int page = 1, int pageSize = 12)
        {
            var brands = _unitOfWork.Brands.GetAll();
            ViewBag.Brands = brands;

            var drinks = _unitOfWork.Drinks.GetAll(includeProperties: "Brand");

            if (!string.IsNullOrEmpty(brand) && brand != "Все бренды")
            {
                drinks = drinks.Where(d => d.Brand.Name == brand);
            }

            var minPriceValue = drinks.Min(d => d.Price);
            var maxPriceValue = drinks.Max(d => d.Price);

            ViewBag.MinPrice = minPriceValue;
            ViewBag.MaxPrice = maxPriceValue;

            if (minPrice.HasValue)
            {
                drinks = drinks.Where(d => d.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                drinks = drinks.Where(d => d.Price <= maxPrice.Value);
            }

            if (sortBy == "Price")
            {
                drinks = drinks.OrderBy(d => d.Price);
            }
            else if (sortBy == "Name")
            {
                drinks = drinks.OrderBy(d => d.Name);
            }

            // Пагинация
            var totalItems = drinks.Count();
            var drinksToDisplay = drinks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalItems = totalItems;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            ViewBag.CartItemCount = HttpContext.Session.GetInt32(StaticDetails.SessionCart) ?? 0;

            return View(drinksToDisplay);
        }

     
        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] ShoppingCart shoppingCart)
        {
            var drink = _unitOfWork.Drinks.GetById(d => d.DrinkId == shoppingCart.DrinkId);

            if (drink == null || drink.Quantity <= 0)
            {
                return Json(new { success = false, message = "Товара нет в наличии" });
            }

            var cartItem = _unitOfWork.ShoppingCarts.GetById(c => c.DrinkId == shoppingCart.DrinkId);
            if (cartItem == null)
            {
                cartItem = new ShoppingCart
                {
                    DrinkId = shoppingCart.DrinkId,
                    Count = 1
                };
                _unitOfWork.ShoppingCarts.Add(cartItem);
            }
            else
            {
                if (cartItem.Count < drink.Quantity)
                {
                    cartItem.Count++;
                }
                else
                {
                    return Json(new { success = false, message = "Нельзя добавить больше товаров, чем есть в наличии" });
                }
            }

            _unitOfWork.Save();

            // Получение актуального количества товаров в корзине
            var cartItems = _unitOfWork.ShoppingCarts.GetAll();
            var cartItemCount = cartItems.Sum(c => c.Count);

            // Обновление значения в сессии
            HttpContext.Session.SetInt32(StaticDetails.SessionCart, cartItemCount);


            return Json(new { success = true, message = "Товар добавлен в корзину", cartItemCount });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPriceRange(string brand)
        {
            var drinks = _unitOfWork.Drinks.GetAll(includeProperties: "Brand");

            if (!string.IsNullOrEmpty(brand) && brand != "Все бренды")
            {
                drinks = drinks.Where(d => d.Brand.Name == brand);
            }

            var minPriceValue = drinks.Min(d => d.Price);
            var maxPriceValue = drinks.Max(d => d.Price);

            return Json(new { minPrice = minPriceValue, maxPrice = maxPriceValue });
        }

    }
}