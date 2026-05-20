using UniDesk.Web.DTOs;
using UniDesk.Web.Models;

namespace UniDesk.Web.Services
{
    public interface ITicketService
    {
        List<Ticket> GetAllTickets();

        List<Ticket> GetTicketsForView(
            string? status,
            string? search,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize);

        Ticket? GetTicketById(int id);

        Ticket AddTicket(Ticket ticket);

        TicketReadDto CreateTicket(CreateTicketRequest request);

        TicketReadDto UpdateTicket(int id, UpdateTicketRequest request);

        List<TicketReadDto> GetAllTicketDtos();

        bool DeleteTicket(int id);

        void UpdateTicketStatus(int id, TicketStatus status);

        PagedResult<TicketListDto> GetTickets(TicketQueryParameters queryParams);
    }
}
