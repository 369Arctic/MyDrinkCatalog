namespace DrinkCatalog.Data.Models.ViewModels
{
    public class PaymentViewModel
    {
        public decimal CartTotal { get; set; } // Итоговая сумма товаров
        public decimal InsertedAmount { get; set; } // Внесенная пользователем сумма
        public List<Coin> Coins { get; set; } // Список доступных монет
        public Dictionary<int, int> Change { get; set; } // Сдача в виде номинала и количества монет
        public string Message { get; set; } // Сообщение об успехе или ошибке

        public Dictionary<int, int> CoinCounts { get; set; }
    }

}
