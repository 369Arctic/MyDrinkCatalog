namespace DrinkCatalog.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IBrandRepository Brands { get; }
        IDrinkRepository Drinks { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        ICoinRepository Coins { get; }
        IOrderRepository Orders { get; }
        void Save();
    }
}
