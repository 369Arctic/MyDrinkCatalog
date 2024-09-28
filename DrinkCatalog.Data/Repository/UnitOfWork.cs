using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;

namespace DrinkCatalog.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Brands = new Repository<Brand>(_dbContext);
            Drinks = new Repository<Drink>(_dbContext);
            ShoppingCarts = new Repository<ShoppingCart>(_dbContext);
            Coins = new Repository<Coin>(_dbContext);
            Orders = new Repository<Order>(_dbContext);
        }

        public IRepository<Brand> Brands { get; set; }
        public IRepository<Drink> Drinks { get; set; }
        public IRepository<ShoppingCart> ShoppingCarts { get; set; }
        public IRepository<Coin> Coins { get; set; }
        public IRepository<Order> Orders { get; set; }

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
