using System.ComponentModel.DataAnnotations;

namespace UniDesc.Web.DTOs
{
    public class UpdateTicketStatusRequest
    {
        [Required]
        public required string Status { get; set; }
    }
}