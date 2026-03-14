using Domain.Events;
using Domain.Shared;

namespace Domain.Tickets;

public class Ticket
{
    public Guid Id { get; private set; }
    public Guid PricingTierId { get; private set; }
    public PricingTier PricingTier { get; private set; } = null!;
    public int CostPaidInCents { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;

    public static Ticket Create(PricingTier pricingTier, int costInCents, string customerEmail)
    {
        return new Ticket
        {
            PricingTier = pricingTier,
            PricingTierId = pricingTier.Id,
            CostPaidInCents = costInCents,
            CustomerEmail = customerEmail
        };
    }
}