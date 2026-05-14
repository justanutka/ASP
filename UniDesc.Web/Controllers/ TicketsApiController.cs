using Microsoft.AspNetCore.Mvc;
using UniDesc.Web.DTOs;
using UniDesc.Web.Models;
using UniDesc.Web.Services;

namespace UniDesc.Web.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    [Tags("Tickets")]
    public class TicketsApiController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsApiController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: api/tickets
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketReadDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TicketReadDto>> GetAllTickets()
        {
            return Ok(_ticketService.GetTicketSummaries());
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TicketReadDto> GetTicketById(int id)
        {
            var ticket = _ticketService.GetTicketReadById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
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

            try
            {
                var dto = _ticketService.CreateTicket(request);

                return CreatedAtAction(nameof(GetTicketById), new { id = dto.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(request.Status), ex.Message);
                return ValidationProblem(ModelState);
            }
        }

        // PATCH: api/tickets/{id}/status
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(TicketReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var ticket = _ticketService.UpdateTicketStatus(id, request.Status);
                if (ticket == null)
                {
                    return NotFound();
                }

                return Ok(ticket);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(request.Status), ex.Message);
                return ValidationProblem(ModelState);
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
