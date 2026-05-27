using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.DTOs
{
    public class CreateTicketRequest
    {
        [Required(ErrorMessage = "Tytul jest wymagany")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tytul musi miec od 3 do 100 znaków")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Status jest wymagany")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Status musi miec od 3 do 20 znaków")]
        public required string Status { get; set; }
    }
}