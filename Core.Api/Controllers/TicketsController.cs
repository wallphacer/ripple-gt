using Core.Application.DTO.Requests;
using Core.Application.Tickets.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok();
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseTickets([FromBody] PurchaseTicketRequest request)
    {
        if (request.EventId == Guid.Empty)
        {
            return BadRequest("EventId cannot be Empty");
        }

        if (request.PricingTierId == Guid.Empty)
        {
            return BadRequest("PricingTierId cannot be Empty");
        }

        var result = await _ticketService.PurchaseTickets(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpGet("availability/{id}")]
    public async Task<IActionResult> GetTicketAvailability(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Id cannot be Empty");
        }

        var result = await _ticketService.GetTicketAvailability(id);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}