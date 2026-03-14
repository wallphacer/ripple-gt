using Domain.Shared;
using Domain.Tickets;

namespace Core.Application.DTO.Responses;

public record PurchasedTicketsResponse(List<PurchasedTicket> PurchasedTickets)
{
    public static PurchasedTicketsResponse ToExternal(List<Ticket> tickets)
    {
        var purchasedTickets = tickets.Select(PurchasedTicket.ToExternal).ToList();
        return new PurchasedTicketsResponse(purchasedTickets);
    }
}

public record PurchasedTicket(
    string EventName,
    string PricingTierName,
    int CostInCents,
    decimal DisplayCost)
{
    public static PurchasedTicket ToExternal(Ticket ticket)
    {
        return new PurchasedTicket(ticket.PricingTier.Event.Name,
            ticket.PricingTier.Name,
            ticket.CostPaidInCents,
            ticket.CostPaidInCents / 100m);
    }
}

public record TicketAvailability(List<PricingTierAvailability> TierAvailabilities)
{
    public static TicketAvailability ToExternal(List<PricingTier> pricingTiers)
    {
        var pricingTierAvailabilities = pricingTiers.Select(PricingTierAvailability.ToExternal).ToList();
        return new TicketAvailability(pricingTierAvailabilities);
    }
}

public record PricingTierAvailability(int CapacityLeft, string Name)
{
    public static PricingTierAvailability ToExternal(PricingTier tier)
    {
        return new PricingTierAvailability(tier.AvailableTickets, tier.Name);
    }
}