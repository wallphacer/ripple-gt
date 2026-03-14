using Core.Application.DTO.Requests;
using Core.Application.DTO.Responses;
using Core.Application.Events;
using Domain.Shared;

namespace Core.Application.Tickets.Services;

public class TicketService : ITicketService
{
    private readonly IEventsRepository _eventsRepository;

    public TicketService(IEventsRepository eventsRepository)
    {
        _eventsRepository = eventsRepository;
    }

    public async Task<Result<PurchasedTicketsResponse>> PurchaseTickets(PurchaseTicketRequest request)
    {
        var purchasedEvent = await _eventsRepository.GetByIdAsync(request.EventId);

        if (purchasedEvent == null)
        {
            return Result<PurchasedTicketsResponse>.Fail($"Could not find Event with Id={request.EventId}");
        }

        var pricingTier = purchasedEvent.PricingTiers.FirstOrDefault(t => t.Id == request.PricingTierId);

        if (pricingTier == null)
        {
            return Result<PurchasedTicketsResponse>.Fail($"Could not find Pricing Tier with Id={request.PricingTierId}");
        }

        var result = pricingTier.PurchaseTickets(request.CustomerEmail, request.Quantity);

        if (!result.IsSuccess)
        {
            return Result<PurchasedTicketsResponse>.Fail(result.Error!);
        }

        await _eventsRepository.UpdateAsync(purchasedEvent);

        return Result<PurchasedTicketsResponse>.Success(PurchasedTicketsResponse.ToExternal(result.Value!));
    }

    public async Task<Result<TicketAvailability>> GetTicketAvailability(Guid id)
    {
        var purchasedEvent = await _eventsRepository.GetByIdAsync(id);

        if (purchasedEvent == null)
        {
            return Result<TicketAvailability>.Fail($"Could not find Event with Id={id}");
        }

        return Result<TicketAvailability>.Success(TicketAvailability.ToExternal(purchasedEvent.PricingTiers.ToList()));

    }
}