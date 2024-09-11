using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DrinkCatalog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Простейшая проверка логина и пароля
            if (username == "admin" && password == "admin123")
            {
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin") // Устанавливаем роль Admin
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

               
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            // Если логин или пароль неверны
            ModelState.AddModelError("", "Неправильный логин или пароль");
            return View();
        }

        public IActionResult Logout()
        {
            // Выполняем выход пользователя
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
