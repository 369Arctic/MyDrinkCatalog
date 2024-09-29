using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrinkCatalog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public IActionResult Index()
        {
            var brands = _brandService.GetAllBrands();
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
                TempData["SuccessMessage"] = _brandService.CreateBrand(brand);
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        public IActionResult EditBrand(int id)
        {
            var brand = _brandService.GetBrandById(id);
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
                TempData["SuccessMessage"] = _brandService.EditBrand(brand);
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        public IActionResult DeleteBrand(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var brandForDelete = _brandService.GetBrandById(id);
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
            TempData["SuccessMessage"] = _brandService.DeleteBrand(id);
            return RedirectToAction("Index");
        }
    }
}
