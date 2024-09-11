using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Data.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);
    }
}
