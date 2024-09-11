using DrinkCatalog.Data.Repository.IRepository;

namespace DrinkCatalog.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Brands = new BrandRepository(_dbContext);
            Drinks = new DrinkRepository(_dbContext);
            ShoppingCarts = new ShoppingCartRepository(_dbContext);
            Coins = new CoinRepository(_dbContext);
            Orders = new OrderRepository(_dbContext);
        }

        public IBrandRepository Brands { get; set; }
        public IDrinkRepository Drinks { get; set; }
        public IShoppingCartRepository ShoppingCarts { get; set; }
        public ICoinRepository Coins { get; set; }
        public IOrderRepository Orders { get; set; }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
