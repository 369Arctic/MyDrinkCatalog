using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Services.IService
{
    public interface IBrandService
    {
        IEnumerable<Brand> GetAllBrands();
        Brand GetBrandById(int id);
        string CreateBrand(Brand brand);
        string EditBrand(Brand brand);
        string DeleteBrand(int id);
    }
}
