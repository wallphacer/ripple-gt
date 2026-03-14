using System.ComponentModel.DataAnnotations;

namespace Core.Application.DTO.Requests;

public record CreateEventRequest(
    [Required][StringLength(200, MinimumLength = 2)] string Name,
    [Required][StringLength(1000, MinimumLength = 2)] string Description,
    [Required][StringLength(300, MinimumLength = 2)] string Venue,
    [Required] DateTime StartTime,
    [Required] int TotalCapacity,
    [Required][MinLength(1)] List<PricingTierDetails> PricingTiers);

public record UpdateEventRequest(
    [Required] Guid Id,
    string? Name,
    string? Description,
    string? Venue,
    DateTime? StartTime,
    int? TotalCapacity,
    List<PricingTierDetails>? PricingTiers);

public record PricingTierDetails(
    [Required][StringLength(100, MinimumLength = 1)] string Name,
    [Required] int CostInCents,
    [Required] int Capacity);