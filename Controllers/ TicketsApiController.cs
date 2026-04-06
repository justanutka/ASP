using Microsoft.AspNetCore.Mvc;
using UniDesc.Web.Models;
using UniDesc.Web.DTOs;
using UniDesc.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace UniDesc.Web.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsApiController : ControllerBase
    {
        private readonly UniDeskDbContext _context;
        private readonly ITicketService _ticketService;

        public TicketsApiController(UniDeskDbContext context, ITicketService ticketService)
        {
            _context = context;
            _ticketService = ticketService;
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
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest("Title and Status are required.");
            }

            try
            {
                var ticket = new Ticket
                {
                    Title = request.Title,
                    Status = Enum.Parse<TicketStatus>(request.Status, true),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
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
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while saving the data: {ex.Message}");
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid status value.");
            }
        }

        // PATCH: api/tickets/{id}/status
        [HttpPatch("{id}/status")]
        public IActionResult UpdateTicketStatus(int id, UpdateTicketStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest("Status is required.");
            }

            try
            {
                var ticket = _context.Tickets.Find(id);

                if (ticket == null)
                {
                    return NotFound();
                }

                ticket.Status = Enum.Parse<TicketStatus>(request.Status, true);
                ticket.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while saving the data: {ex.Message}");
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid status value.");
            }
        }

        // GET: api/tickets/search
        [HttpGet("search")]
        public ActionResult<IEnumerable<TicketListDto>> GetTickets([FromQuery] TicketQueryParameters queryParams)
        {
            try
            {
                var tickets = _ticketService.GetTickets(queryParams).ToList();
                return Ok(tickets);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}