using System;

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
