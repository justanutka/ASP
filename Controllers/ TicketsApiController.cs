using Microsoft.AspNetCore.Mvc;
using UniDesc.Web.Models;
using UniDesc.Web.DTOs;
using Microsoft.EntityFrameworkCore;

namespace UniDesc.Web.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsApiController : ControllerBase
    {
        private readonly UniDeskDbContext _context;

        public TicketsApiController(UniDeskDbContext context)
        {
            _context = context;
        }

        // GET: api/tickets
        [HttpGet]
        public ActionResult<IEnumerable<TicketReadDto>> GetAllTickets()
        {
            var tickets = _context.Tickets.ToList();
            var ticketDtos = tickets.Select(t => new TicketReadDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status.ToString()
            });

            return Ok(ticketDtos);
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        public ActionResult<TicketReadDto> GetTicketById(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var ticketDto = new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };

            return Ok(ticketDto);
        }

        // POST: api/tickets
        [HttpPost]
        public ActionResult<TicketReadDto> CreateTicket(CreateTicketRequest request)
        {
            var ticket = new Ticket
            {
                Title = request.Title,
                Status = Enum.Parse<TicketStatus>(request.Status)
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();  

            var dto = new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };

            return CreatedAtAction(nameof(GetTicketById), new { id = dto.Id }, dto);
        }

        // PATCH: api/tickets/{id}/status
        [HttpPatch("{id}/status")]
        public IActionResult UpdateTicketStatus(int id, UpdateTicketStatusRequest request)
        {
            var ticket = _context.Tickets.Find(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = Enum.Parse<TicketStatus>(request.Status, true);
            _context.SaveChanges();

            return NoContent();
        }
    }
}