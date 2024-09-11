using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DrinkCatalog.Data.Models
{
    public class Drink
    {
        public int DrinkId { get; set; }

        [Required(ErrorMessage = "Введите название напитка")]
        [StringLength(255, ErrorMessage = "Название напитка не может превышать 255 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите цену напитка")]
        [Range(1, int.MaxValue, ErrorMessage = "Цена должна быть больше 1")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Введите количество напитков")]
        [Range(0, 30, ErrorMessage = "Колличество напитков должно располагаться в диапазоне 0 - 30")]
        public int Quantity { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }
        public int BrandId { get; set; }
        [ValidateNever]
        public Brand Brand { get; set; }

    }
}
