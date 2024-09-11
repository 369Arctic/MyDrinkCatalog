using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BrandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            var brands = _unitOfWork.Brands.GetAll();
            return View(brands);
        }

        public IActionResult CreateBrand()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateBrand(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Brands.Add(brand);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Бренд успешно создан";
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        public IActionResult EditBrand(int id)
        {
            var brand = _unitOfWork.Brands.GetById(u => u.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost]
        public IActionResult EditBrand(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Brands.Update(brand);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Бренд успешно изменен";
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        public IActionResult DeleteBrand(int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var brandForDelete = _unitOfWork.Brands.GetById(u => u.BrandId == id);
            if (brandForDelete == null)
            {
                return NotFound();
            }
            return View(brandForDelete);
        }

        [HttpPost]
        [ActionName("DeleteBrand")]
        public IActionResult DeleteBrandPost(int id)
        {
            var brand = _unitOfWork.Brands.GetById(u => u.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }
            _unitOfWork.Brands.Remove(brand);
            _unitOfWork.Save();
            TempData["SuccessMessage"] = "Бренд успешно удален";
            return RedirectToAction("Index");
        }

    }
}
