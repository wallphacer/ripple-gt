using Domain.Events;

public record EventResponse(
    Guid Id,
    string Name,
    string Description,
    string Venue,
    DateTime StartTime,
    int TotalCapacity)
{
    public static EventResponse ToExternal(Event ev)
    {
        return new EventResponse(
            ev.Id,
            ev.Name,
            ev.Description,
            ev.Venue,
            ev.EventDate,
            ev.TotalCapacity
        );
    }
}