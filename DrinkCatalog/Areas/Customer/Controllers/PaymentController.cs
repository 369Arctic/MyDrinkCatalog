using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Utility;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var cartItems = _unitOfWork.ShoppingCarts.GetAll(includeProperties: "Drink").ToList();
            var cartTotal = cartItems.Sum(x => x.Drink.Price * x.Count);

            var coins = _unitOfWork.Coins.GetAll()
                                         .OrderBy(u => u.Denomination)
                                         .ToList();

            var paymentVM = new PaymentViewModel
            {
                CartTotal = cartTotal,
                Coins = coins,
                InsertedAmount = 0, // Начальная сумма внесенных монет
                Change = new Dictionary<int, int>()
            };

            return View(paymentVM);
        }

        // Обработка оплаты
        [HttpPost]
        public IActionResult ProcessPayment(PaymentViewModel paymentVM, string coinCounts)
        {
            var coins = _unitOfWork.Coins.GetAll()
                                          .OrderBy(u => u.Denomination)
                                          .ToList();

            var coinCountsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(coinCounts);

            // Вычисляем сдачу
            var change = CalculateChange(paymentVM.CartTotal, paymentVM.InsertedAmount, coins);

            if (change == null)
            {
                paymentVM.Message = "Извините, в данный момент мы не можем продать вам товар по причине того, что автомат не может выдать вам нужную сдачу";
                paymentVM.Coins = coins;
                return View("Index", paymentVM);
            }

            //  Обновляем количество монет, внесенных пользователем
            foreach (var coin in coinCountsDict)
            {
                var coinInSystem = coins.FirstOrDefault(c => c.Denomination == coin.Key);
                if (coinInSystem != null)
                {
                    coinInSystem.Count += coin.Value; 
                    _unitOfWork.Coins.Update(coinInSystem);
                }
            }

            _unitOfWork.Save();

            //  Обновляем количество монет в системе для сдачи
            foreach (var coin in change)
            {
                var coinInSystem = coins.FirstOrDefault(c => c.Denomination == coin.Key);
                if (coinInSystem != null)
                {
                    coinInSystem.Count -= coin.Value; 
                    _unitOfWork.Coins.Update(coinInSystem);
                }
            }

            // Сохраняем изменения после вычитания монет для сдачи
            _unitOfWork.Save();

            // Обработка заказа
            var order = new Order
            {
                OrderDate = DateTime.Now.ToUniversalTime(),
                OrderTotal = paymentVM.CartTotal,
                OrderDetails = new List<OrderDetail>()
            };

            var cartItems = _unitOfWork.ShoppingCarts.GetAll(includeProperties: "Drink.Brand").ToList();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Brand = item.Drink.Brand.Name,
                    Name = item.Drink.Name,
                    Quantity = item.Count,
                    Price = item.Drink.Price
                };
                order.OrderDetails.Add(orderDetail);
                var drinkInDb = _unitOfWork.Drinks.GetFirstOrDefault(d => d.DrinkId == item.DrinkId);
                if (drinkInDb != null)
                {
                    drinkInDb.Quantity -= item.Count; // Уменьшаем количество на количество купленных товаров
                    _unitOfWork.Drinks.Update(drinkInDb); 
                }
            }

            _unitOfWork.Orders.Add(order);
            _unitOfWork.Save();

            var userShoppingCart = _unitOfWork.ShoppingCarts.GetAll().ToList();
            foreach (var item in userShoppingCart)
            {
                _unitOfWork.ShoppingCarts.Remove(item);
            }
            _unitOfWork.Save();

            paymentVM.Message = "Спасибо за покупку! Ваша сдача:";
            paymentVM.Change = change;
            HttpContext.Session.SetInt32(StaticDetails.SessionCart, 0);

            return View("Success", paymentVM);
        }

        private Dictionary<int, int> CalculateChange(decimal totalCost, decimal insertedAmount, List<Coin> availableCoins)
        {
            if (insertedAmount < totalCost)
            {
                return null; // Недостаточно средств
            }

            var changeAmount = insertedAmount - totalCost;
            var change = new Dictionary<int, int>();

            foreach (var coin in availableCoins.OrderByDescending(c => c.Denomination))
            {
                if (changeAmount == 0)
                {
                    break;
                }

                var count = (int)(changeAmount / coin.Denomination);
                if (count > 0 && coin.Count > 0)
                {
                    var coinsToGive = count <= coin.Count ? count : coin.Count;
                    change[coin.Denomination] = coinsToGive;
                    changeAmount -= coinsToGive * coin.Denomination;
                }
            }

            return changeAmount == 0 ? change : null; // Если не удалось дать сдачу точной суммой
        }
    }
}
