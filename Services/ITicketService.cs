using UniDesc.Web.Models;

namespace UniDesc.Web.Services
{
    public interface ITicketService
    {
        List<Ticket> GetAllTickets();
        void AddTicket(Ticket ticket);
        Ticket? GetTicketById(int id);
        void UpdateTicketStatus(int id, TicketStatus status);
        System.Linq.IQueryable<UniDesc.Web.DTOs.TicketListDto> GetTickets(TicketQueryParameters queryParams);
    }
}