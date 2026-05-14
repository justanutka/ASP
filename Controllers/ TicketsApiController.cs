using Microsoft.AspNetCore.Mvc;
using UniDesc.Web.DTOs;
using UniDesc.Web.Models;
using UniDesc.Web.Services;

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
            var tickets = _ticketService.GetAllTicketDtos();

            return Ok(tickets);
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

        // POST: api/tickets
        [HttpPost]
        public ActionResult<TicketReadDto> CreateTicket(CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var createdTicket = _ticketService.CreateTicket(request);

                return CreatedAtAction(
                    nameof(GetTicketById),
                    new { id = createdTicket.Id },
                    createdTicket);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(request.Status), ex.Message);
                return ValidationProblem(ModelState);
            }
        }

        // PATCH: api/tickets/{id}/status
        [HttpPatch("{id}/status")]
        public IActionResult UpdateTicketStatus(int id, UpdateTicketStatusRequest request)
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

            var ticket = _ticketService.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.UpdateTicketStatus(id, parsedStatus);

            var updatedTicket = _ticketService.GetTicketById(id);

            var dto = new TicketReadDto
            {
                Id = updatedTicket!.Id,
                Title = updatedTicket.Title,
                Status = updatedTicket.Status.ToString()
            };

            return Ok(dto);
        }

        // GET: api/tickets/search
        [HttpGet("search")]
        public ActionResult<PagedResult<TicketListDto>> GetTickets([FromQuery] TicketQueryParameters queryParams)
        {
            try
            {
                var result = _ticketService.GetTickets(queryParams);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}