using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Data.Repository.IRepository
{
    public interface IBrandRepository : IRepository<Brand>
    {
        void Update(Brand brand);
    }
}
