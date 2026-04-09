using UniDesc.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace UniDesc.Web.Services
{
    public class InMemoryTicketService
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

            if (ticket == null)
            {
                throw new InvalidOperationException("Ticket not found.");
            }

            if (ticket.Status == TicketStatus.Closed && status == TicketStatus.Closed)
            {
                throw new InvalidOperationException("Ticket is already closed.");
            }

            ticket.Status = status;
        }

        //GetTickets
        public IQueryable<Ticket> GetTickets(TicketQueryParameters queryParams)
        {
            var query = _tickets.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Status))
            {
                query = query.Where(t => t.Status.ToString().Equals(queryParams.Status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(queryParams.SortBy) && queryParams.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(t => t.CreatedAt);
            }

            query = query.Skip((queryParams.Page - 1) * queryParams.PageSize)
                         .Take(queryParams.PageSize);

            return query;
        }
    }
}