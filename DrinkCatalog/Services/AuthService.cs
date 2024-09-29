using DrinkCatalog.Services.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace DrinkCatalog.Services
{
    public class AuthService : IAuthService
    {
        public bool Authenticate(string username, string password)
        {
            return username == "admin" && password == "admin123";
        }

        public ClaimsPrincipal CreatePrincipal(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
    }
}
