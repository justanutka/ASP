using System;

namespace UniDesc.Web.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

        public TicketStatus Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public enum TicketStatus
    {
        New,
        InProgress,
        Closed
    }
}