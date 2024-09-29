using DrinkCatalog.Data.Models;
using DrinkCatalog.Data.Repository.IRepository;
using DrinkCatalog.Services.IService;

namespace DrinkCatalog.Services
{
    public class CoinService : ICoinService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoinService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Coin> GetAllCoins()
        {
            return _unitOfWork.Coins.GetAll().OrderBy(u => u.Denomination).ToList();
        }

        public Coin GetCoinById(int id)
        {
            return _unitOfWork.Coins.GetById(u => u.Id == id);
        }

        public string AddOrUpdateCoin(Coin coin)
        {
            int[] allowedDenominations = { 1, 2, 5, 10 };
            if (!allowedDenominations.Contains(coin.Denomination))
            {
                return "Номинал монеты должен быть 1, 2, 5 или 10 рублей.";
            }

            var existingCoin = _unitOfWork.Coins.GetFirstOrDefault(u => u.Denomination == coin.Denomination);
            if (existingCoin != null)
            {
                existingCoin.Count += coin.Count;
                _unitOfWork.Coins.Update(existingCoin);
                _unitOfWork.Save();
                return "Количество монет успешно обновлено";
            }
            else
            {
                _unitOfWork.Coins.Add(coin);
                _unitOfWork.Save();
                return "Новая монета успешно добавлена";
            }
        }

        public string EditCoin(Coin coin)
        {
            _unitOfWork.Coins.Update(coin);
            _unitOfWork.Save();
            return "Монета успешно отредактирована";
        }

    }
}
