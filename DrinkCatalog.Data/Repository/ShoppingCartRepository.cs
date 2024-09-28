using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Data.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>
    {
        private ApplicationDbContext _dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
