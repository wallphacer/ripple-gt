using System.ComponentModel.DataAnnotations;

namespace Core.Application.DTO.Requests;

public record PurchaseTicketRequest(
    [Required] Guid EventId,
    [Required] Guid PricingTierId,
    [Required] int Quantity,
    [Required][StringLength(256, MinimumLength = 5)] string CustomerEmail);
