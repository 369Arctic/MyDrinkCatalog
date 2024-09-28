namespace DrinkCatalog.Data.Models.ViewModels
{
    public class DrinkCatalogVM
    {
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<Drink> Drinks { get; set; }

        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int CartItemCount { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
