using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;

namespace DrinkCatalog.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Brand> GetAllBrands()
        {
            return _unitOfWork.Brands.GetAll();
        }

        public Brand GetBrandById(int id)
        {
            return _unitOfWork.Brands.GetById(u => u.BrandId == id);
        }

        public string CreateBrand(Brand brand)
        {
            _unitOfWork.Brands.Add(brand);
            _unitOfWork.Save();
            return "Бренд успешно создан";
        }

        public string EditBrand(Brand brand)
        {
            _unitOfWork.Brands.Update(brand);
            _unitOfWork.Save();
            return "Бренд успешно изменен";
        }

        public string DeleteBrand(int id)
        {
            var brand = _unitOfWork.Brands.GetById(u => u.BrandId == id);
            if (brand == null)
            {
                return "Бренд не найден";
            }
            _unitOfWork.Brands.Remove(brand);
            _unitOfWork.Save();
            return "Бренд успешно удален";
        }
    }
}
