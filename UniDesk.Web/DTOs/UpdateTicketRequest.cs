using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.DTOs
{
    public class UpdateTicketRequest
    {
        [Required(ErrorMessage = "Tytul jest wymagany")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Tytul musi miec od 3 do 60 znaków")]
        public required string Title { get; set; }

        [StringLength(200, ErrorMessage = "Opis moze miec maksymalnie 200 znaków")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Status jest wymagany")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Status musi miec od 3 do 20 znaków")]
        public required string Status { get; set; }
    }
}