using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;

namespace DrinkCatalog.Data.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext _dbContext;
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(Order order)
        {
            _dbContext.Orders.Update(order);
        }
    }
}
