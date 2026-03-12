using UniDesc.Web.Models;

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

        public Ticket GetTicketById(int id)
        {
            var ticket = _tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }
            return ticket;
        }
    }
}