using System.ComponentModel.DataAnnotations;

namespace UniDesc.Web.DTOs
{
    public class CreateTicketRequest
    {
        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [Required]
        public required string Status { get; set; }
    }
}