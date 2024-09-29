using System.Security.Claims;

namespace DrinkCatalog.Services.IService
{
    public interface IAuthService
    {
        bool Authenticate(string username, string password);
        ClaimsPrincipal CreatePrincipal(string username);
    }
}
