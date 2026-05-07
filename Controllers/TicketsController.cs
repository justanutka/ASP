using UniDesc.Web.Models;
using UniDesc.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace UniDesc.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IActionResult Index(string? status, string? sortBy, string? sortDirection, int page = 1, int pageSize = 10)
        {
            var tickets = _ticketService.GetAllTickets().AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<TicketStatus>(status, true, out var parsedStatus))
                {
                    tickets = tickets.Where(t => t.Status == parsedStatus);
                }
                else
                {
                    ModelState.AddModelError("Status", "Niepoprawny status.");
                }
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

            var result = tickets
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(result);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            _ticketService.AddTicket(ticket);

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var ticket = _ticketService.GetAllTickets()
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
    }
}