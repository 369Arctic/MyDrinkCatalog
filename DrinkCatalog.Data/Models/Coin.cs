using System.ComponentModel.DataAnnotations;

namespace DrinkCatalog.Data.Models
{
    public class Coin
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите номинал монеты")]
        [Range(1, int.MaxValue, ErrorMessage = "Номинал монеты не может быть отрицательным")]
        public int Denomination { get; set; }

        [Required(ErrorMessage = "Введите количество доступных монет")]
        [Range(0, 100, ErrorMessage = "Количество монет в аппарате может быть от 0 до 100")]
        public int Count { get; set; }
    }
}
