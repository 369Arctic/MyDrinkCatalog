using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Services.IService
{
    public interface ICoinService
    {
        IEnumerable<Coin> GetAllCoins();
        Coin GetCoinById(int id);
        string AddOrUpdateCoin(Coin coin);
        string EditCoin(Coin coin);
    }
}
