using System;

namespace Domain.Events;

public class Event
{
    public Guid Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public int TotalCapacity { get; set; }

    public static Event Create(string name, string description, string venue, DateTime eventDate, int totalCapacity)
    {
        return new Event
        {
            Name = name,
            Description = description,
            Venue = venue,
            EventDate = eventDate,
            TotalCapacity = totalCapacity
        };
    }
}