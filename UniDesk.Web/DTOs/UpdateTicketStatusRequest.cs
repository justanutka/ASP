using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.DTOs
{
    public class UpdateTicketStatusRequest
    {
        [Required(ErrorMessage = "Status jest wymagany")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Status musi miec od 3 do 20 znaków")]
        public required string Status { get; set; }
    }
}