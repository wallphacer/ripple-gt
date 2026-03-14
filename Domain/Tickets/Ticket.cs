using Domain.Events;
using Domain.Shared;

namespace Domain.Tickets;

public class Ticket
{
    public Guid Id { get; private set; }
    public int CostPaidInCents { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;

    public static Ticket Create(int costInCents, string customerEmail)
    {
        return new Ticket
        {
            CostPaidInCents = costInCents,
            CustomerEmail = customerEmail
        };
    }
}