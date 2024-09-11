using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace DrinkCatalog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DrinkController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DrinkImportService _drinkImportService;
        public DrinkController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, DrinkImportService drinkImportService)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _drinkImportService = drinkImportService;
        }


        public IActionResult Index()
        {
            var drinks = _unitOfWork.Drinks.GetAll(includeProperties: "Brand").ToList();
            return View(drinks);
        }

        public IActionResult CreateDrink()
        {
            IEnumerable<SelectListItem> BrandList = _unitOfWork.Brands.
               GetAll().Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.BrandId.ToString()
               });
            DrinkVM drinkVM = new()
            {
                Drink = new Drink(),
                BrandList = BrandList
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
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string drinkPath = Path.Combine(_webHostEnvironment.WebRootPath, @"Img\Drink");

                    if (!Directory.Exists(drinkPath))
                    {
                        Directory.CreateDirectory(drinkPath);
                    }

                    using (var image = Image.Load(file.OpenReadStream()))
                    {
                        int targetWidth = 300;
                        int targetHeight = 300;

                        image.Mutate(x => x.Resize(targetWidth, targetHeight));

                        var encoder = GetEncoder(file.FileName);

                        string fullPath = Path.Combine(drinkPath, fileName);
                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            image.Save(fileStream, encoder);
                        }
                    }

                    drinkVM.Drink.ImageUrl = @"\Img\Drink\" + fileName;
                }
                else
                {
                    TempData["ErrorMessage"] = "Добавьте изображение";
                    return View();
                }
                _unitOfWork.Drinks.Add(drinkVM.Drink);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Напиток успешно создан";
                return RedirectToAction("Index");
            }
            return View(drinkVM);
        }

        public IActionResult EditDrink(int id)
        {
            IEnumerable<SelectListItem> BrandList = _unitOfWork.Brands.
                GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.BrandId.ToString()
                });
            DrinkVM drinkVM = new()
            {
                Drink = new Drink(),
                BrandList = BrandList
            };

            drinkVM.Drink = _unitOfWork.Drinks.GetById(u => u.DrinkId == id);
            return View(drinkVM);

        }


        [HttpPost]
        public IActionResult EditDrink(DrinkVM drinkVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string drinkPath = Path.Combine(_webHostEnvironment.WebRootPath, @"Img\Drink");

                    // Если есть старое изображение, то удаляем его
                    if (!string.IsNullOrEmpty(drinkVM.Drink.ImageUrl))
                    {

                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, drinkVM.Drink.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var image = Image.Load(file.OpenReadStream()))
                    {
                        int targetWidth = 300;
                        int targetHeight = 300;

                        image.Mutate(x => x.Resize(targetWidth, targetHeight));

                        var encoder = GetEncoder(file.FileName);

                        string fullPath = Path.Combine(drinkPath, fileName);
                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            image.Save(fileStream, encoder);
                        }
                    }

                    drinkVM.Drink.ImageUrl = @"\Img\Drink\" + fileName;
                }
                _unitOfWork.Drinks.Update(drinkVM.Drink);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Напиток успешно изменен";
                return RedirectToAction("Index");
            }
            return View(drinkVM);
        }


        public IActionResult DeleteDrink(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var drinkForDelete = _unitOfWork.Drinks.GetById(u => u.DrinkId == id);
            if (drinkForDelete == null)
            {
                return NotFound();
            }
            return View(drinkForDelete);
        }

        [HttpPost]
        public IActionResult DeleteDrink(int id)
        {
            var drinkForDelete = _unitOfWork.Drinks.GetById(u => u.DrinkId == id);
            if (drinkForDelete == null)
            {
                return NotFound();
            }
            _unitOfWork.Drinks.Remove(drinkForDelete);
            _unitOfWork.Save();
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
                    _unitOfWork.Drinks.Add(drink);
                }
                _unitOfWork.Save();

                TempData["SuccessMessage"] = "Товары успешно импортированы.";
                return RedirectToAction("Index");
            }
        }

        private IImageEncoder GetEncoder(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".png" => new PngEncoder(),
                ".jpg" or ".jpeg" => new JpegEncoder(),
                ".bmp" => new BmpEncoder(),
                _ => throw new NotSupportedException("Unsupported image format"),
            }; ;
        }
    }
}
