using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;

namespace DrinkCatalog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DrinkController : Controller
    {
        private readonly IDrinkImportService _drinkImportService;
        private readonly IDrinkService _drinkService;
        private readonly IImageService _imageService;
        private readonly IBrandService _brandService;

        public DrinkController(IDrinkService drinkService, IImageService imageService, IDrinkImportService drinkImportService, IBrandService brandService)
        {
            _drinkService = drinkService;
            _imageService = imageService;
            _drinkImportService = drinkImportService;
            _brandService = brandService;
        }


        public IActionResult Index()
        {
            var drinks = _drinkService.GetAllDrinks();
            return View(drinks);
        }

        public IActionResult CreateDrink()
        {
            IEnumerable<SelectListItem> brandList = _brandService.GetAllBrands()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.BrandId.ToString()
                    });

            DrinkVM drinkVM = new()
            {
                Drink = new Drink(),
                BrandList = brandList
            };
            return View(drinkVM);
        }

        [HttpPost]
        public IActionResult CreateDrink(DrinkVM drinkVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    drinkVM.Drink.ImageUrl = _imageService.SaveImage(file, @"Img\Drink");
                }
                else
                {
                    TempData["ErrorMessage"] = "Добавьте изображение";
                    return View(drinkVM);
                }
                _drinkService.AddDrink(drinkVM.Drink);
                TempData["SuccessMessage"] = "Напиток успешно создан";
                return RedirectToAction("Index");
            }
            return View(drinkVM);
        }

        public IActionResult EditDrink(int id)
        {
            IEnumerable<SelectListItem> brandList = _brandService.GetAllBrands()
                   .Select(u => new SelectListItem
                   {
                       Text = u.Name,
                       Value = u.BrandId.ToString()
                   });

            var drinkVM = new DrinkVM
            {
                Drink = _drinkService.GetDrinkById(id),
                BrandList = brandList
            };
            return View(drinkVM);
        }


        [HttpPost]
        public IActionResult EditDrink(DrinkVM drinkVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    _imageService.DeleteImage(drinkVM.Drink.ImageUrl);
                    drinkVM.Drink.ImageUrl = _imageService.SaveImage(file, @"Img\Drink");
                }
                _drinkService.UpdateDrink(drinkVM.Drink);
                TempData["SuccessMessage"] = "Напиток успешно изменен";
                return RedirectToAction("Index");
            }
            return View(drinkVM);
        }


        public IActionResult DeleteDrink(int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var drinkForDelete = _drinkService.GetDrinkById(id);
            if (drinkForDelete == null)
            {
                return NotFound();
            }
            return View(drinkForDelete);
        }

        [HttpPost]
        [ActionName("DeleteDrink")]
        public IActionResult DeleteDrinkPost(int id)
        {
            _drinkService.DeleteDrink(id);
            TempData["SuccessMessage"] = "Напиток успешно удален";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ImportDrinks(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Выберите файл для загрузки.";
                return RedirectToAction("Index");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var (drinks, errors) = await _drinkImportService.ImportDrinksAsync(stream);

                if (errors.Any())
                {
                    TempData["ErrorMessage"] = string.Join("<br/>", errors);
                    return RedirectToAction("Index");
                }

                foreach (var drink in drinks)
                {
                    _drinkService.AddDrink(drink);
                }

                TempData["SuccessMessage"] = "Товары успешно импортированы.";
                return RedirectToAction("Index");
            }
        }
    }
}
