using System.ComponentModel.DataAnnotations;

namespace UniDesc.Web.DTOs
{
    public class UpdateTicketRequest
    {
        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Tytuł musi mieć od 3 do 60 znaków")]
        public required string Title { get; set; }

        [StringLength(200, ErrorMessage = "Opis może mieć maksymalnie 200 znaków")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Status jest wymagany")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Status musi mieć od 3 do 20 znaków")]
        public required string Status { get; set; }
    }
}