using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Utility;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Отображение корзины
        public IActionResult Index()
        {
            var cartItems = _unitOfWork.ShoppingCarts.GetAll(includeProperties: "Drink").ToList();

            if (!cartItems.Any())
            {
                TempData["Message"] = "У вас нет ни одного товара, вернитесь на страницу каталога.";
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, 0); // Обновление сессии при пустой корзине
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
            var cartItem = _unitOfWork.ShoppingCarts.GetById(c => c.Id == cartId);

            if (cartItem != null)
            {
                _unitOfWork.ShoppingCarts.Remove(cartItem);
                _unitOfWork.Save();
                var cartItems = _unitOfWork.ShoppingCarts.GetAll().ToList();
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, cartItems.Sum(x => x.Count));
            }

            return RedirectToAction(nameof(Index));
        }

        // Обновление количества товара
        [HttpPost]
        public IActionResult UpdateCart(int cartId, int newCount)
        {
            var cartItem = _unitOfWork.ShoppingCarts.GetById(c => c.Id == cartId, includeProperties: "Drink");
            string errorMessage = null;

            if (cartItem != null)
            {
                if (newCount <= 0)
                {
                    _unitOfWork.ShoppingCarts.Remove(cartItem);
                }
                else if (newCount <= cartItem.Drink.Quantity)
                {
                    cartItem.Count = newCount;
                    _unitOfWork.ShoppingCarts.Update(cartItem);
                }
                else
                {
                    // Устанавливаем сообщение об ошибке
                    errorMessage = $"Невозможно добавить {newCount} шт. {cartItem.Drink.Name}. Доступно только {cartItem.Drink.Quantity} шт.";
                }

                _unitOfWork.Save();
                var cartItems = _unitOfWork.ShoppingCarts.GetAll().ToList();
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, cartItems.Sum(x => x.Count));
            }

            // Возвращаем результат в формате JSON с возможным сообщением об ошибке
            return Json(new { success = string.IsNullOrEmpty(errorMessage), errorMessage = errorMessage });
        }

        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            var cartItems = _unitOfWork.ShoppingCarts.GetAll().ToList();
            var cartItemCount = cartItems.Sum(x => x.Count); // Суммируем количество товаров
            return Json(new { cartItemCountt = cartItemCount });
        }

        // Методы, которые нужны в случае, если требуется удалять товар из корзины,
        // когда пользователь уменьшает количества товара, при этом текущее кол-во = 1
        //public IActionResult Plus(int cartId)
        //{
        //    var cartFromDb = _unitOfWork.ShoppingCarts.GetById(u => u.Id == cartId);
        //    cartFromDb.Count += 1;
        //    _unitOfWork.ShoppingCarts.Update(cartFromDb);
        //    _unitOfWork.Save();
        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult Minus (int cartId)
        //{
        //    var cartFromDb = _unitOfWork.ShoppingCarts.GetById(u => u.Id == cartId);
        //    if(cartFromDb.Count <= 1)
        //    {
        //        _unitOfWork.ShoppingCarts.Remove(cartFromDb);
        //    }
        //    else
        //    {
        //        cartFromDb.Count -= 1;
        //        _unitOfWork.ShoppingCarts.Update(cartFromDb);
        //    }
        //    _unitOfWork.Save();
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
