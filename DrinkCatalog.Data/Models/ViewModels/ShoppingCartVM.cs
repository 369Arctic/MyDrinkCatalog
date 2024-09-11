namespace DrinkCatalog.Data.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartsList { get; set; }
        public Order Order { get; set; }
    }
}
