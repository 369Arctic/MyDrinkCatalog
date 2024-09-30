using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;

namespace DrinkCatalog.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PaymentViewModel PreparePaymentViewModel()
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

            return paymentVM;
        }

        public PaymentViewModel ProcessPayment(PaymentViewModel paymentVM, Dictionary<int, int> coinCounts)
        {
            var coins = _unitOfWork.Coins.GetAll().OrderBy(u => u.Denomination).ToList();

            // Вычисляем сдачу
            var change = CalculateChange(paymentVM.CartTotal, paymentVM.InsertedAmount, coins);

            if (change == null)
            {
                paymentVM.Message = "Извините, автомат не может выдать нужную сдачу.";
                paymentVM.Coins = coins;
                return paymentVM;
            }

            // Обновляем количество монет, внесенных пользователем
            foreach (var coin in coinCounts)
            {
                var coinInSystem = coins.FirstOrDefault(c => c.Denomination == coin.Key);
                if (coinInSystem != null)
                {
                    coinInSystem.Count += coin.Value;
                    _unitOfWork.Coins.Update(coinInSystem);
                }
            }

            _unitOfWork.Save();

            // Обновляем количество монет для сдачи
            foreach (var coin in change)
            {
                var coinInSystem = coins.FirstOrDefault(c => c.Denomination == coin.Key);
                if (coinInSystem != null)
                {
                    coinInSystem.Count -= coin.Value;
                    _unitOfWork.Coins.Update(coinInSystem);
                }
            }

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
                    drinkInDb.Quantity -= item.Count;
                    _unitOfWork.Drinks.Update(drinkInDb);
                }
            }

            _unitOfWork.Orders.Add(order);
            _unitOfWork.Save();

            // Очищаем корзину
            var userShoppingCart = _unitOfWork.ShoppingCarts.GetAll().ToList();
            foreach (var item in userShoppingCart)
            {
                _unitOfWork.ShoppingCarts.Remove(item);
            }

            _unitOfWork.Save();

            paymentVM.Message = "Спасибо за покупку! Ваша сдача:";
            paymentVM.Change = change;
            return paymentVM;
        }

        public Dictionary<int, int> CalculateChange(decimal totalCost, decimal insertedAmount, List<Coin> availableCoins)
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

            return changeAmount == 0 ? change : null;
        }
    }
}
