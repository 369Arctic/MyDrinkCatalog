using DrinkCatalog.Data.Models.ViewModels;
using DrinkCatalog.Data.Models;

namespace DrinkCatalog.Services.IService
{
    public interface IPaymentService
    {
        PaymentViewModel ProcessPayment(PaymentViewModel paymentVM, Dictionary<int, int> coinCounts);
        Dictionary<int, int> CalculateChange(decimal totalCost, decimal insertedAmount, List<Coin> availableCoins);
        PaymentViewModel PreparePaymentViewModel();
    }
}
