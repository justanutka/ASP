using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podaj poprawny adres email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Haslo jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasla jest wymagane.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Hasla nie sa takie same.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}