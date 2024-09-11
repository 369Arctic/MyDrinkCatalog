using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;

namespace DrinkCatalog.Data.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private ApplicationDbContext _dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _dbContext.ShoppingCarts.Update(shoppingCart);
        }
    }
}
