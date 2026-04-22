using Microsoft.AspNetCore.Mvc;
using UniDesc.Web.Models;
using UniDesc.Web.DTOs;
using UniDesc.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace UniDesc.Web.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    [Tags("Tickets")]
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
        [ProducesResponseType(typeof(IEnumerable<TicketReadDto>), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(TicketReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(typeof(TicketReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<TicketReadDto> CreateTicket([FromBody] CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (!Enum.TryParse<TicketStatus>(request.Status, true, out var parsedStatus))
            {
                ModelState.AddModelError(nameof(request.Status), "Invalid status value.");
                return ValidationProblem(ModelState);
            }

            try
            {
                var ticket = new Ticket
                {
                    Title = request.Title,
                    Status = parsedStatus,
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
        }

        // PATCH: api/tickets/{id}/status
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                ModelState.AddModelError(nameof(request.Status), "Status is required.");
                return ValidationProblem(ModelState);
            }

            if (!Enum.TryParse<TicketStatus>(request.Status, true, out var parsedStatus))
            {
                ModelState.AddModelError(nameof(request.Status), "Invalid status value.");
                return ValidationProblem(ModelState);
            }

            try
            {
                var ticket = _context.Tickets.Find(id);

                if (ticket == null)
                {
                    return NotFound();
                }

                ticket.Status = parsedStatus;
                ticket.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while saving the data: {ex.Message}");
            }
        }

        // GET: api/tickets/search
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<TicketListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<PagedResult<TicketListDto>> GetTickets([FromQuery] TicketQueryParameters queryParams)
        {
            try
            {
                var result = _ticketService.GetTickets(queryParams);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("queryParams", ex.Message);
                return ValidationProblem(ModelState);
            }
        }
    }
}