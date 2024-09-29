using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Services.IService
{
    public interface IDrinkService
    {
        IEnumerable<Drink> GetAllDrinks();
        void AddDrink(Drink drink);
        void UpdateDrink(Drink drink);
        void DeleteDrink(int drinkId);
        Drink GetDrinkById(int drinkId);
    }
}
