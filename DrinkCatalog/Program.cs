using DrinkCatalog.Data;
using DrinkCatalog.Data.Repository;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services;
using DrinkCatalog.Services.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICoinService, CoinService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IDrinkImportService, DrinkImportService>();
builder.Services.AddScoped<IDrinkService, DrinkService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();




//builder.Services.AddScoped<DrinkImportService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Admin/Account/Login"; // Путь к странице входа для администратора
                });


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); 
}

app.UseHttpsRedirection(); 
app.UseStaticFiles(); 
app.UseSession();

app.UseRouting(); 


app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "{area=Admin}/{controller=Account}/{action=Login}/{id?}");


app.Run();
