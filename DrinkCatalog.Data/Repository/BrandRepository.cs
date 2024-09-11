using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;

namespace DrinkCatalog.Data.Repository
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private ApplicationDbContext _dbContext;
        public BrandRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(Brand brand)
        {
            _dbContext.Brands.Update(brand);
        }
    }
}
