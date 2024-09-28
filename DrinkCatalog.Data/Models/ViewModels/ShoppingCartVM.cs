namespace DrinkCatalog.Data.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartsList { get; set; }
        public Order Order { get; set; }

        public decimal CartTotal { get; set; }
        public int CartItemCount { get; set; }
        public string Message { get; set; }
    }
}
