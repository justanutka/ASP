using UniDesc.Web.Models;
using UniDesc.Web.DTOs;

namespace UniDesc.Web.Services
{
    public class TicketService : ITicketService
    {
        private readonly UniDeskDbContext _context;

        public TicketService(UniDeskDbContext context)
        {
            _context = context;
        }

        public List<Ticket> GetAllTickets()
        {
            return _context.Tickets.ToList();
        }

        public void AddTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }

        public Ticket? GetTicketById(int id)
        {
            return _context.Tickets.Find(id);
        }

        public void UpdateTicketStatus(int id, TicketStatus status)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket != null)
            {
                ticket.Status = status;
                ticket.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }

        public PagedResult<TicketListDto> GetTickets(TicketQueryParameters queryParams)
        {
            var query = _context.Tickets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Status))
            {
                if (Enum.TryParse(queryParams.Status, true, out TicketStatus status))
                {
                    query = query.Where(t => t.Status == status);
                }
                else
                {
                    throw new ArgumentException(
                        $"Niepoprawna wartość statusu: {queryParams.Status}",
                        nameof(queryParams.Status)
                    );
                }
            }

            var allowedSortFields = new[] { "createdat", "title", "status" };

            var sortBy = queryParams.SortBy?.Trim().ToLower();
            var sortDirection = queryParams.SortDirection?.Trim().ToLower();

            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "createdat";
            }

            if (!allowedSortFields.Contains(sortBy))
            {
                throw new ArgumentException(
                    $"Sortowanie po polu '{queryParams.SortBy}' nie jest dozwolone.",
                    nameof(queryParams.SortBy)
                );
            }

            query = sortBy switch
            {
                "title" => sortDirection == "desc"
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title),

                "status" => sortDirection == "desc"
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),

                _ => sortDirection == "desc"
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt)
            };

            int totalCount = query.Count();

            int page = queryParams.Page < 1 ? 1 : queryParams.Page;
            int pageSize = queryParams.PageSize < 1 ? 10 : queryParams.PageSize;

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TicketListDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status.ToString()
                })
                .ToList();

            return new PagedResult<TicketListDto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}