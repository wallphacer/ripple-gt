using Domain.Events;
using Domain.Tickets;

namespace Domain.Shared;

public class PricingTier
{
    public Guid Id { get; set; }

    public int CostInCents { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid EventId { get; set; }

    public Event Event { get; set; } = null!;

    public int Capacity { get; set; }

    public int TicketsSold { get; set; }

    public int Version { get; set; }

    private readonly List<Ticket> _tickets = new();

    public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    // Derived Values
    public int AvailableTickets => Capacity - TicketsSold;

    public static PricingTier Create(string name, int costInCents, int capacity, Event relatedEvent)
    {
        return new PricingTier
        {
            CostInCents = costInCents,
            Name = name,
            EventId = relatedEvent.Id,
            Event = relatedEvent,
            Capacity = capacity,
            TicketsSold = 0
        };
    }

    public Result<List<Ticket>> PurchaseTickets(string customerEmail, int quantity)
    {
        if (TicketsSold + quantity > Capacity)
        {
            return Result<List<Ticket>>.Fail($"{Name} has sold {TicketsSold} tickets, so cannot sell {quantity}.");
        }

        var tickets = new List<Ticket>();
        for (int i = 0; i < quantity; i++)
        {
            var newTicket = Ticket.Create(this, CostInCents, customerEmail);
            _tickets.Add(newTicket);
            tickets.Add(newTicket);
        }

        TicketsSold += quantity;

        // Concurrency Control happens here!
        Version++;
        return Result<List<Ticket>>.Success(tickets);
    }
}