using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podaj poprawny adres email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Haslo jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}