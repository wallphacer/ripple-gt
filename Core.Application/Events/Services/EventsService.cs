using Core.Application.DTO.Requests;
using Core.Application.DTO.Responses;
using Domain.Events;
using Domain.Shared;

namespace Core.Application.Events.Services;

public class EventsService : IEventsService
{
    private readonly IEventsRepository _eventsRepository;

    public EventsService(IEventsRepository eventsRepository)
    {
        _eventsRepository = eventsRepository;
    }

    public async Task<Result<IList<EventResponse>>> GetAllEvents()
    {
        var events = await _eventsRepository.GetAllEvents();

        return Result<IList<EventResponse>>.Success(events.Select(EventResponse.ToExternal).ToList());
    }

    public async Task<Result<EventResponse>> GetEventById(Guid id)
    {
        var foundEvent = await _eventsRepository.GetByIdAsync(id);

        return foundEvent != null ?
            Result<EventResponse>.Success(EventResponse.ToExternal(foundEvent!))
            : Result<EventResponse>.Fail($"Could not find Event by Id={id}");
    }

    public async Task<Result> DeleteEventById(Guid id)
    {
        var success = await _eventsRepository.DeleteAsync(id);

        return success ? Result.Success() : Result.Fail($"Could not delete resource with Id={id}");
    }

    public async Task<Result<EventResponse>> UpdateEvent(Guid id, UpdateEventRequest request)
    {
        var existingEvent = await _eventsRepository.GetByIdAsync(id);

        if (existingEvent == null)
        {
            return Result<EventResponse>.Fail($"Could not find Event with Id={id}");
        }

        existingEvent.Name = request.Name ?? existingEvent.Name;
        existingEvent.Description = request.Description ?? existingEvent.Description;
        existingEvent.Venue = request.Venue ?? existingEvent.Venue;
        existingEvent.EventDate = request.StartTime ?? existingEvent.EventDate;

        if (request.TotalCapacity.HasValue)
        {
            existingEvent.TotalCapacity = request.TotalCapacity.Value;
        }

        //TODO: Update Pricing Tiers (will require a merge)

        await _eventsRepository.UpdateAsync(existingEvent);

        return Result<EventResponse>.Success(EventResponse.ToExternal(existingEvent));
    }

    public async Task<Result<EventResponse>> AddEvent(CreateEventRequest request)
    {
        var createdEvent = Event.Create(request.Name,
            request.Description,
            request.Venue,
            request.StartTime,
            request.TotalCapacity);

        foreach (var tier in request.PricingTiers)
        {
            var result = createdEvent.AddPricingTier(tier.Name, tier.CostInCents, tier.Capacity);

            if (!result.IsSuccess)
            {
                return Result<EventResponse>.Fail(result.Error!);
            }
        }

        await _eventsRepository.AddAsync(createdEvent);

        return Result<EventResponse>.Success(EventResponse.ToExternal(createdEvent));
    }
}