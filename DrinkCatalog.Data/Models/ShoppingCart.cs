using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinkCatalog.Data.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int DrinkId { get; set; }
        [ForeignKey("DrinkId")]
        [ValidateNever]
        public Drink Drink { get; set; }
        public int Count { get; set; }

    }
}
