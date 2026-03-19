using System;
using System.ComponentModel.DataAnnotations;

namespace UniDesc.Web.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Tytuł musi mieć od 3 do 60 znaków")]
        public string Title { get; set; } = "";

        [StringLength(200, ErrorMessage = "Opis może mieć maksymalnie 200 znaków")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Status jest wymagany")]
        public TicketStatus Status { get; set; } = TicketStatus.New;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum TicketStatus
    {
        New,
        InProgress,
        Closed
    }
}