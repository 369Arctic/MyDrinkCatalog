using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;

namespace DrinkCatalog.Data.Repository
{
    public class CoinRepository : Repository<Coin>, ICoinRepository
    {
        private ApplicationDbContext _dbContext;

        public CoinRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Coin coin)
        {
            _dbContext.Coins.Update(coin);
        }
    }
}
