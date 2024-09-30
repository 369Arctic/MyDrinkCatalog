using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;

namespace DrinkCatalog.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CatalogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Drink> GetFilteredDrinks(string brand, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var drinks = _unitOfWork.Drinks.GetAll(includeProperties: "Brand");

            if (!string.IsNullOrEmpty(brand) && brand != "Все бренды")
            {
                drinks = drinks.Where(d => d.Brand.Name == brand);
            }

            if (minPrice.HasValue)
            {
                drinks = drinks.Where(d => d.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                drinks = drinks.Where(d => d.Price <= maxPrice.Value);
            }

            drinks = sortBy == "Price" ? drinks.OrderBy(d => d.Price) : drinks.OrderBy(d => d.Name);

            return drinks;
        }

        public decimal GetMaxPrice()
        {
            return _unitOfWork.Drinks.GetAll().Max(d => d.Price);
        }

        public decimal GetMinPrice()
        {
            return _unitOfWork.Drinks.GetAll().Min(d => d.Price);
        }

        
        public DrinkCatalogVM GetDrinkCatalogViewModel(string brand, decimal? minPrice, decimal? maxPrice, string sortBy, int page, int pageSize)
        {
            var drinks = GetFilteredDrinks(brand, minPrice, maxPrice, sortBy);
            var brands = drinks.Select(u => u.Brand).Distinct().ToList();

            var minPriceValue = GetMinPrice();
            var maxPriceValue = GetMaxPrice();

            var totalItems = drinks.Count();
            var drinksToDisplay = drinks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new DrinkCatalogVM
            {
                Brands = brands,
                Drinks = drinksToDisplay,
                MinPrice = minPriceValue,
                MaxPrice = maxPriceValue,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
