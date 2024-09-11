using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Data.Repository.IRepository
{
    public interface ICoinRepository : IRepository<Coin>
    {
        void Update(Coin coin);
    }
}
