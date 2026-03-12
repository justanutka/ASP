using Microsoft.AspNetCore.Mvc;
using UniDesc.Web.Models;
using UniDesc.Web.Services;
using UniDesc.Web.DTOs;

namespace UniDesc.Web.Controllers
{
    [ApiController]  
    [Route("api/tickets")] 
    public class TicketsApiController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsApiController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: api/tickets
        [HttpGet]
        public ActionResult<IEnumerable<TicketReadDto>> GetAllTickets()
        {
            var tickets = _ticketService.GetAllTickets();
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
            var ticket = _ticketService.GetTicketById(id);
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

        //new ticket
        [HttpPost]
        public ActionResult<TicketReadDto> CreateTicket(CreateTicketRequest request)
        {
            var ticket = new Ticket
            {
                Title = request.Title,
                Status = Enum.Parse<TicketStatus>(request.Status)
            };

            _ticketService.AddTicket(ticket);

            var dto = new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };

            return CreatedAtAction(nameof(GetTicketById), new { id = dto.Id }, dto);
        }

        //PATCH endpoint
        [HttpPatch("{id}/status")]
        public IActionResult UpdateTicketStatus(int id, UpdateTicketStatusRequest request)
        {
            var ticket = _ticketService.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = Enum.Parse<TicketStatus>(request.Status, true);

            return NoContent();
        }
    }
}