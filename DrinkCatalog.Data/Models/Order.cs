namespace DrinkCatalog.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderTotal { get; set; }
        public ICollection<OrderDetail> OrderDetails{ get; set; }
    }
}
