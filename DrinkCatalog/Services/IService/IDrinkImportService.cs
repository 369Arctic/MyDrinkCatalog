using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Services.IService
{
    public interface IDrinkImportService
    {
        Task<(List<Drink> drinks, List<string> errors)> ImportDrinksAsync(Stream fileStream);
    }
}
