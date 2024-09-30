using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;
using DrinkCatalog.Utility;

namespace DrinkCatalog.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public (bool success, string message) AddToCart(ShoppingCart shoppingCart)
        {
            var drink = _unitOfWork.Drinks.GetById(d => d.DrinkId == shoppingCart.DrinkId);
            string message = null;

            if (drink == null || drink.Quantity <= 0)
            {
                message = "Ошибка при добавлении в корзину";
            }

            var cartItem = _unitOfWork.ShoppingCarts.GetById(c => c.DrinkId == shoppingCart.DrinkId);
            if (cartItem == null)
            {
                cartItem = new ShoppingCart { DrinkId = shoppingCart.DrinkId, Count = 1 };
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
                    message = $"Ошибка при добавлении в корзину. " +
                        $"Невозможно добавить {drink.Quantity+1} шт. {cartItem.Drink.Name}. Доступно только {cartItem.Drink.Quantity} шт.";
                }
            }

            _unitOfWork.Save();
            return (string.IsNullOrEmpty(message), message);
        }

        public void RemoveFromCart(int cartId)
        {
            var cartItem = _unitOfWork.ShoppingCarts.GetById(c => c.Id == cartId);
            if (cartItem != null)
            {
                _unitOfWork.ShoppingCarts.Remove(cartItem);
                _unitOfWork.Save();
            }
        }

        public (bool success, string errorMessage) UpdateCartItemCount(int cartId, int newCount)
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
                    errorMessage = $"Невозможно добавить {newCount} шт. {cartItem.Drink.Name}. Доступно только {cartItem.Drink.Quantity} шт.";
                }

                _unitOfWork.Save();
            }

            return (string.IsNullOrEmpty(errorMessage), errorMessage);
        }

        public IEnumerable<ShoppingCart> GetAllCartItems()
        {
            return _unitOfWork.ShoppingCarts.GetAll(includeProperties: "Drink");
        }

        public int GetCartItemCount()
        {
            return _unitOfWork.ShoppingCarts.GetAll().Sum(c => c.Count);
        }

        public (bool success, string message, int cartItemCount) AddToCartAndUpdateSession(ShoppingCart shoppingCart, ISession session)
        {
            var (success, message) = AddToCart(shoppingCart);
            var cartItemCount = GetCartItemCount();
            session.SetInt32(StaticDetails.SessionCart, cartItemCount);

            return (success, message, cartItemCount);
        }
    }
}
