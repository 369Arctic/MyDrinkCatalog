using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Data.Repository.IRepository
{
    public interface IDrinkRepository : IRepository<Drink>
    {
        void Update(Drink drink);
    }
}
