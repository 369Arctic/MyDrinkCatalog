using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;

namespace DrinkCatalog.Services
{
    public class DrinkService : IDrinkService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DrinkService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void AddDrink(Drink drink)
        {
            _unitOfWork.Drinks.Add(drink);
            _unitOfWork.Save();
        }

        public void DeleteDrink(int drinkId)
        {
            var drink = GetDrinkById(drinkId);
            if (drink != null)
            {
                _unitOfWork.Drinks.Remove(drink);
                _unitOfWork.Save();
            }
        }

        public IEnumerable<Drink> GetAllDrinks()
        {
            return _unitOfWork.Drinks.GetAll(includeProperties: "Brand").ToList();
        }

        public Drink GetDrinkById(int drinkId)
        {
            return _unitOfWork.Drinks.GetById(u => u.DrinkId == drinkId);
        }

        public void UpdateDrink(Drink drink)
        {
            _unitOfWork.Drinks.Update(drink);
            _unitOfWork.Save();
        }
    }
}
