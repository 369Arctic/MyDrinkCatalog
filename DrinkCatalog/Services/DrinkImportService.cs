using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using OfficeOpenXml;
using System.Globalization;

namespace DrinkCatalog.Services
{
    public class DrinkImportService
    {
        public async Task<(List<Drink> drinks, List<string> errors)> ImportDrinksAsync(Stream fileStream)
        {
            var drinks = new List<Drink>();
            var errors = new List<string>();

            // Устанавливаем контекст лицензии, без него падает в ошибку.
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var name = worksheet.Cells[row, 1].Text;
                    var priceString = worksheet.Cells[row, 2].Text;
                    var quantityString = worksheet.Cells[row, 3].Text;
                    var imageUrl = worksheet.Cells[row, 4].Text;
                    var brandIdString = worksheet.Cells[row, 5].Text;

                    if (string.IsNullOrWhiteSpace(name) || !decimal.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) ||
                        !int.TryParse(quantityString, out var quantity) || !int.TryParse(brandIdString, out var brandId))
                    {
                        errors.Add($"Ошибка на строке {row}: Неверные данные.");
                        continue;
                    }

                    if (price <= 0 || quantity < 0 || quantity > 30)
                    {
                        errors.Add($"Ошибка на строке {row}: Некорректные значения цены или количества.");
                        continue;
                    }

                    var drink = new Drink
                    {
                        Name = name,
                        Price = price,
                        Quantity = quantity,
                        ImageUrl = imageUrl, 
                        BrandId = brandId
                    };

                    drinks.Add(drink);
                }
            }

            return (drinks, errors);
        }
    }
}
