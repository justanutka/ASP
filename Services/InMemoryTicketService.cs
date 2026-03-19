using UniDesc.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace UniDesc.Web.Services
{
    public class InMemoryTicketService : ITicketService
    {
        private readonly List<Ticket> _tickets = new(); 

        public List<Ticket> GetAllTickets()
        {
            return _tickets; 
        }

        public void AddTicket(Ticket ticket)
        {
            ticket.Id = _tickets.Count + 1;  
            _tickets.Add(ticket);
        }

        public Ticket? GetTicketById(int id)
        {
            return _tickets.FirstOrDefault(t => t.Id == id);  
        }

        public void UpdateTicketStatus(int id, TicketStatus status)
        {
            var ticket = _tickets.FirstOrDefault(t => t.Id == id); 
            if (ticket != null)
            {
                ticket.Status = status;  
            }
        }
    }
}