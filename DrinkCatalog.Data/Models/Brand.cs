using System.ComponentModel.DataAnnotations;

namespace DrinkCatalog.Data.Models
{
    public class Brand
    {
        public int BrandId { get; set; }
        [Required(ErrorMessage ="Введите название бренда")]
        public string Name { get; set; }
        public ICollection<Drink> Drinks { get; set; }

        public Brand()
        {
            Drinks = new List<Drink>();
        }
    }
}
