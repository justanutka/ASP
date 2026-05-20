using UniDesk.Web.Models;
using UniDesk.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace UniDesk.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IActionResult Index(
            string? status,
            string? search,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var tickets = _ticketService.GetTicketsForView(status, search, sortBy, sortDirection, page, pageSize);
                return View(tickets);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("Status", "Niepoprawny status.");
                return View(new List<Ticket>());
            }
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
            var ticket = _ticketService.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
    }
}
