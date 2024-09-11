using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrinkCatalog.Data.Models.ViewModels
{
    public class DrinkVM
    {
        public Drink Drink { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> BrandList { get; set; }
    }
}
