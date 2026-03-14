using Core.Application.DTO.Requests;
using Core.Application.DTO.Responses;
using Domain.Shared;

namespace Core.Application.DTO.Tickets.Services;

public interface ITicketsService
{
    Task<Result<PurchasedTicketsResponse>> PurchaseTickets(PurchaseTicketRequest request);
}