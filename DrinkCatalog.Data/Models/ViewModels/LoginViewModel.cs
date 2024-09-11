using System.ComponentModel.DataAnnotations;

namespace DrinkCatalog.Data.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your username.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
