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

        public IReadOnlyList<Ticket> GetTicketsForView(TicketQueryParameters queryParams)
        {
            return ApplyTicketQuery(_context.Tickets.AsQueryable(), queryParams)
                .ToList();
        }

        public IReadOnlyList<TicketReadDto> GetTicketSummaries()
        {
            return _context.Tickets
                .OrderBy(t => t.Id)
                .Select(t => ToTicketReadDto(t))
                .ToList();
        }

        public void AddTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }

        public TicketReadDto CreateTicket(CreateTicketRequest request)
        {
            if (!Enum.TryParse<TicketStatus>(request.Status, true, out var parsedStatus))
            {
                throw new ArgumentException("Invalid status value.", nameof(request.Status));
            }

            var ticket = new Ticket
            {
                Title = request.Title,
                Status = parsedStatus
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return ToTicketReadDto(ticket);
        }

        public Ticket? GetTicketById(int id)
        {
            return _context.Tickets.Find(id);
        }

        public TicketReadDto? GetTicketReadById(int id)
        {
            var ticket = _context.Tickets.Find(id);

            return ticket == null ? null : ToTicketReadDto(ticket);
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

        public TicketReadDto? UpdateTicketStatus(int id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status is required.", nameof(status));
            }

            if (!Enum.TryParse<TicketStatus>(status, true, out var parsedStatus))
            {
                throw new ArgumentException("Invalid status value.", nameof(status));
            }

            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return null;
            }

            ticket.Status = parsedStatus;
            ticket.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return ToTicketReadDto(ticket);
        }

        public bool DeleteTicket(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return false;
            }

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();

            return true;
        }

        public PagedResult<TicketListDto> GetTickets(TicketQueryParameters queryParams)
        {
            var filteredAndSortedQuery = ApplyTicketQuery(_context.Tickets.AsQueryable(), queryParams, applyPaging: false);

            int totalCount = filteredAndSortedQuery.Count();

            int page = queryParams.Page < 1 ? 1 : queryParams.Page;
            int pageSize = queryParams.PageSize < 1 ? 10 : queryParams.PageSize;

            var items = filteredAndSortedQuery
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

        private static IQueryable<Ticket> ApplyTicketQuery(
            IQueryable<Ticket> query,
            TicketQueryParameters queryParams,
            bool applyPaging = true)
        {
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

            var sortBy = queryParams.SortBy?.Trim().ToLowerInvariant();
            var sortDirection = queryParams.SortDirection?.Trim().ToLowerInvariant();

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

            if (!applyPaging)
            {
                return query;
            }

            int page = queryParams.Page < 1 ? 1 : queryParams.Page;
            int pageSize = queryParams.PageSize < 1 ? 10 : queryParams.PageSize;

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        private static TicketReadDto ToTicketReadDto(Ticket ticket)
        {
            return new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };
        }
    }
}
