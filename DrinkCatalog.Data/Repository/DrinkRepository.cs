using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;

namespace DrinkCatalog.Data.Repository
{
    public class DrinkRepository : Repository<Drink>, IDrinkRepository
    {
        private ApplicationDbContext _dbContext;
        public DrinkRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(Drink drink)
        {
            _dbContext.Drinks.Update(drink);
        }
    }
}
