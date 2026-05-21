using UniDesk.Web.Models;
using UniDesk.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace UniDesk.Web.Controllers
{
    [Authorize]
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

            try
            {
                _ticketService.AddTicket(ticket);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nie mozna zapisac zgloszenia. Sprawdz dane i sprobuj ponownie.");
                return View(ticket);
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            var ticket = _ticketService.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (!Enum.TryParse<TicketStatus>(status, true, out var parsedStatus))
            {
                ModelState.AddModelError("Status", "Niepoprawny status.");
                return View("Details", ticket);
            }

            try
            {
                _ticketService.UpdateTicketStatus(id, parsedStatus);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nie mozna zaktualizowac statusu. Sprobuj ponownie.");
                return View("Details", ticket);
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
