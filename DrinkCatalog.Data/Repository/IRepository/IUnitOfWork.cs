using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Brand> Brands { get; }
        //IDrinkRepository Drinks { get; }
        IRepository<Drink> Drinks { get; }
        IRepository<ShoppingCart> ShoppingCarts { get; }
        IRepository<Coin> Coins { get; }
        IRepository<Order> Orders { get; }
        void Save();
    }
}
