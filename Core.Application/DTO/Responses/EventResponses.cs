using Domain.Events;
using Domain.Shared;

namespace Core.Application.DTO.Responses;

public record EventResponse(
    Guid Id,
    string Name,
    string Description,
    string Venue,
    DateTime StartTime,
    int TotalCapacity,
    IEnumerable<PricingTierResponse> PricingTiers)
{
    public static EventResponse ToExternal(Event ev)
    {
        return new EventResponse(
            ev.Id,
            ev.Name,
            ev.Description,
            ev.Venue,
            ev.EventDate,
            ev.TotalCapacity,
            ev.PricingTiers.Select(PricingTierResponse.ToExternal)
        );
    }
}

public record PricingTierResponse(
    Guid Id,
    string Name,
    int CostInCents,
    decimal DisplayCost)
{
    public static PricingTierResponse ToExternal(PricingTier pt)
    {
        return new PricingTierResponse(
            pt.Id,
            pt.Name,
            pt.CostInCents,
            pt.CostInCents / 100m
        );
    }
}