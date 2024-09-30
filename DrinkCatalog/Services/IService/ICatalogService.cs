using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Models.ViewModels;

namespace DrinkCatalog.Services.IService
{
    public interface ICatalogService
    {
        IEnumerable<Drink> GetFilteredDrinks(string brand, decimal? minPrice, decimal? maxPrice, string sortBy);
        decimal GetMinPrice();
        decimal GetMaxPrice();
        DrinkCatalogVM GetDrinkCatalogViewModel(string brand, decimal? minPrice, decimal? maxPrice, string sortBy, int page, int pageSize);
    }
}
