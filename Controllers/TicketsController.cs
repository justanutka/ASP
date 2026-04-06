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

        public IActionResult Index(string? status, string? sortBy, int page = 1, int pageSize = 10)
        {
            var queryParams = new TicketQueryParameters
            {
                Status = status,
                SortBy = sortBy,
                Page = page,
                PageSize = pageSize
            };

            var tickets = _ticketService.GetTickets(queryParams).ToList();

            return View(tickets);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
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