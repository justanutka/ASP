using UniDesc.Web.DTOs;
using UniDesc.Web.Models;

namespace UniDesc.Web.Services
{
    public interface ITicketService
    {
        List<Ticket> GetAllTickets();
        IReadOnlyList<Ticket> GetTicketsForView(TicketQueryParameters queryParams);
        IReadOnlyList<TicketReadDto> GetTicketSummaries();
        void AddTicket(Ticket ticket);
        TicketReadDto CreateTicket(CreateTicketRequest request);
        Ticket? GetTicketById(int id);
        TicketReadDto? GetTicketReadById(int id);
        void UpdateTicketStatus(int id, TicketStatus status);
        TicketReadDto? UpdateTicketStatus(int id, string status);
        bool DeleteTicket(int id);
        PagedResult<UniDesc.Web.DTOs.TicketListDto> GetTickets(TicketQueryParameters queryParams);
    }
}
