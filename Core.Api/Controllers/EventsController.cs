using Core.Application.DTO;
using Core.Application.Events.Services;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventsService _eventsService;

        public EventsController(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { Timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<ActionResult<List<EventResponse>>> GetEvents()
        {
            var result = await _eventsService.GetAllEvents();

            // Returning this because it will either be a full list
            // Or Default to an empty response (which for a GetAll is better, as it doesn't reveal anything
            // about the internal size of the data set
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponse>> GetEvent(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Id Requested");
            }

            var result = await _eventsService.GetEventById(id);

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Error!);
        }

        [HttpPost]
        public async Task<ActionResult<EventResponse>> CreateEvent([FromBody] CreateEventRequest request)
        {
            var result = await _eventsService.AddEvent(request);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetEvent), new { id = result.Value!.Id }, result.Value);
            }

            return BadRequest(result.Error!);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EventResponse>> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
        {
            // Bit weird to have both ids, but I consider this a type of guard
            // Are you trying to update what you are claiming
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Id Requested");
            }

            if (id != request.Id)
            {
                return BadRequest($"Id mismatch, provided both {id} and {request.Id}");
            }

            var result = await _eventsService.UpdateEvent(request.Id, request);

            return result.IsSuccess ? Ok(result.Value!) : NotFound(result.Error!);
        }
    }
}
