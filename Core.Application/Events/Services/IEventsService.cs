
using Core.Application.DTO.Requests;
using Core.Application.DTO.Responses;
using Domain.Shared;

namespace Core.Application.Events.Services;

public interface IEventsService
{
    public Task<Result<IList<EventResponse>>> GetAllEvents();


    public Task<Result<EventResponse>> GetEventById(Guid id);

    public Task<Result> DeleteEventById(Guid id);

    public Task<Result<EventResponse>> UpdateEvent(Guid id, UpdateEventRequest eventToChange);

    public Task<Result<EventResponse>> AddEvent(CreateEventRequest request);

}