using DrinkCatalog.Data.Models;
using Microsoft.EntityFrameworkCore;
namespace DrinkCatalog.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Drink> Drinks { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Coin> Coins { get; set; }

}
