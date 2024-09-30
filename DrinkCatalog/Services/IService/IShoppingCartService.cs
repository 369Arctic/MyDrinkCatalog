using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Services.IService
{
    public interface IShoppingCartService
    {
        public (bool success, string message) AddToCart(ShoppingCart shoppingCart); // через кортеж, чтобы была возможность обрабатывать сообщения.
        void RemoveFromCart(int cartId);
        public (bool success, string errorMessage) UpdateCartItemCount(int cartId, int newCount);
        public (bool success, string message, int cartItemCount) AddToCartAndUpdateSession(ShoppingCart shoppingCart, ISession session);
        IEnumerable<ShoppingCart> GetAllCartItems();
        int GetCartItemCount();
    }
}
