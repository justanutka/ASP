using System.Diagnostics;
using UniDesk.Web.DTOs;
using UniDesk.Web.Exceptions;
using UniDesk.Web.Models;

namespace UniDesk.Web.Services
{
    public class TicketService : ITicketService
    {
        private const long SlowOperationThresholdMs = 10;

        private readonly UniDeskDbContext _context;
        private readonly ILogger<TicketService> _logger;

        public TicketService(UniDeskDbContext context, ILogger<TicketService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Ticket> GetAllTickets()
        {
            return _context.Tickets.ToList();
        }

        public List<Ticket> GetTicketsForView(
            string? status,
            string? search,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize)
        {
            var tickets = _context.Tickets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<TicketStatus>(status, true, out var parsedStatus))
                {
                    tickets = tickets.Where(t => t.Status == parsedStatus);
                }
                else
                {
                    throw new ArgumentException("Niepoprawny status.");
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                tickets = tickets.Where(t => t.Title.Contains(search));
            }

            sortBy = sortBy?.ToLower();
            sortDirection = sortDirection?.ToLower();

            tickets = sortBy switch
            {
                "title" => sortDirection == "desc"
                    ? tickets.OrderByDescending(t => t.Title)
                    : tickets.OrderBy(t => t.Title),

                "status" => sortDirection == "desc"
                    ? tickets.OrderByDescending(t => t.Status)
                    : tickets.OrderBy(t => t.Status),

                "createdat" => sortDirection == "desc"
                    ? tickets.OrderByDescending(t => t.CreatedAt)
                    : tickets.OrderBy(t => t.CreatedAt),

                _ => tickets.OrderBy(t => t.CreatedAt)
            };

            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            return tickets
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Ticket? GetTicketById(int id)
        {
            return _context.Tickets.Find(id);
        }

        public Ticket AddTicket(Ticket ticket)
        {
            var stopwatch = Stopwatch.StartNew();

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            stopwatch.Stop();

            _logger.LogInformation(
                "Ticket {TicketId} created with title {TicketTitle}, status {TicketStatus}, operation {Operation}, module {Module}, elapsed {ElapsedMilliseconds} ms",
                ticket.Id,
                ticket.Title,
                ticket.Status.ToString(),
                "CreateTicket",
                "Tickets",
                stopwatch.ElapsedMilliseconds);

            LogSlowOperationIfNeeded(
                operation: "CreateTicket",
                ticketId: ticket.Id,
                elapsedMilliseconds: stopwatch.ElapsedMilliseconds);

            return ticket;
        }

        public TicketReadDto CreateTicket(CreateTicketRequest request)
        {
            if (!Enum.TryParse<TicketStatus>(request.Status, true, out var status))
            {
                throw new ArgumentException($"Niepoprawna wartosc statusu: {request.Status}");
            }

            var stopwatch = Stopwatch.StartNew();

            var ticket = new Ticket
            {
                Title = request.Title,
                Status = status,
                Description = ""
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            stopwatch.Stop();

            _logger.LogInformation(
                "Ticket {TicketId} created with title {TicketTitle}, status {TicketStatus}, operation {Operation}, module {Module}, elapsed {ElapsedMilliseconds} ms",
                ticket.Id,
                ticket.Title,
                ticket.Status.ToString(),
                "CreateTicket",
                "Tickets",
                stopwatch.ElapsedMilliseconds);

            LogSlowOperationIfNeeded(
                operation: "CreateTicket",
                ticketId: ticket.Id,
                elapsedMilliseconds: stopwatch.ElapsedMilliseconds);

            return new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };
        }

        public TicketReadDto UpdateTicket(int id, UpdateTicketRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            var ticket = _context.Tickets.Find(id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with id {id} was not found.");
            }

            if (!Enum.TryParse<TicketStatus>(request.Status, true, out var status))
            {
                throw new ArgumentException($"Niepoprawna wartosc statusu: {request.Status}");
            }

            ticket.Title = request.Title;
            ticket.Description = request.Description;
            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            stopwatch.Stop();

            _logger.LogInformation(
                "Ticket {TicketId} updated with status {TicketStatus}, operation {Operation}, module {Module}, elapsed {ElapsedMilliseconds} ms",
                ticket.Id,
                ticket.Status.ToString(),
                "UpdateTicket",
                "Tickets",
                stopwatch.ElapsedMilliseconds);

            LogSlowOperationIfNeeded(
                operation: "UpdateTicket",
                ticketId: ticket.Id,
                elapsedMilliseconds: stopwatch.ElapsedMilliseconds);

            return new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };
        }

        public List<TicketReadDto> GetAllTicketDtos()
        {
            return _context.Tickets
                .Select(t => new TicketReadDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status.ToString()
                })
                .ToList();
        }

        public bool DeleteTicket(int id)
        {
            var stopwatch = Stopwatch.StartNew();

            var ticket = _context.Tickets.Find(id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with id {id} was not found.");
            }

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();

            stopwatch.Stop();

            _logger.LogInformation(
                "Ticket {TicketId} deleted, operation {Operation}, module {Module}, elapsed {ElapsedMilliseconds} ms",
                ticket.Id,
                "DeleteTicket",
                "Tickets",
                stopwatch.ElapsedMilliseconds);

            LogSlowOperationIfNeeded(
                operation: "DeleteTicket",
                ticketId: ticket.Id,
                elapsedMilliseconds: stopwatch.ElapsedMilliseconds);

            return true;
        }

        public void UpdateTicketStatus(int id, TicketStatus status)
        {
            var stopwatch = Stopwatch.StartNew();

            var ticket = _context.Tickets.Find(id);

            if (ticket != null)
            {
                ticket.Status = status;
                ticket.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();

                stopwatch.Stop();

                _logger.LogInformation(
                    "Ticket {TicketId} changed status to {TicketStatus}, operation {Operation}, module {Module}, elapsed {ElapsedMilliseconds} ms",
                    ticket.Id,
                    ticket.Status.ToString(),
                    "UpdateTicketStatus",
                    "Tickets",
                    stopwatch.ElapsedMilliseconds);

                LogSlowOperationIfNeeded(
                    operation: "UpdateTicketStatus",
                    ticketId: ticket.Id,
                    elapsedMilliseconds: stopwatch.ElapsedMilliseconds);
            }
        }

        public PagedResult<TicketListDto> GetTickets(TicketQueryParameters queryParams)
        {
            var stopwatch = Stopwatch.StartNew();

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
                        $"Niepoprawna wartosc statusu: {queryParams.Status}",
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

            stopwatch.Stop();

            _logger.LogInformation(
                "Tickets list loaded, operation {Operation}, module {Module}, totalCount {TotalCount}, page {Page}, pageSize {PageSize}, elapsed {ElapsedMilliseconds} ms",
                "GetTickets",
                "Tickets",
                totalCount,
                page,
                pageSize,
                stopwatch.ElapsedMilliseconds);

            if (stopwatch.ElapsedMilliseconds > SlowOperationThresholdMs)
            {
                _logger.LogWarning(
                    "Slow operation detected for {Operation} in module {Module}. Elapsed {ElapsedMilliseconds} ms, threshold {ThresholdMilliseconds} ms",
                    "GetTickets",
                    "Tickets",
                    stopwatch.ElapsedMilliseconds,
                    SlowOperationThresholdMs);
            }

            return new PagedResult<TicketListDto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }

        private void LogSlowOperationIfNeeded(string operation, int ticketId, long elapsedMilliseconds)
        {
            if (elapsedMilliseconds <= SlowOperationThresholdMs)
            {
                return;
            }

            _logger.LogWarning(
                "Slow operation detected for {Operation} in module {Module}. TicketId {TicketId}, elapsed {ElapsedMilliseconds} ms, threshold {ThresholdMilliseconds} ms",
                operation,
                "Tickets",
                ticketId,
                elapsedMilliseconds,
                SlowOperationThresholdMs);
        }
    }
}