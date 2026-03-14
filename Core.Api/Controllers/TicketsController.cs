using Core.Application.DTO.Requests;
using Core.Application.DTO.Tickets.Services;
using Domain.Tickets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{

    private readonly ITicketsService _ticketService;

    public TicketsController(ITicketsService ticketsService)
    {
        _ticketService = ticketsService;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { Timestamp = DateTime.UtcNow });
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
}

