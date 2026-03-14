using Core.Application.DTO.Requests;
using Core.Application.DTO.Responses;
using Domain.Shared;

namespace Core.Application.Tickets.Services;

public interface ITicketService
{
    Task<Result<PurchasedTicketsResponse>> PurchaseTickets(PurchaseTicketRequest request);

    Task<Result<TicketAvailability>> GetTicketAvailability(Guid id);

}