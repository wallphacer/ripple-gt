using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.DTO;

public record CreateEventRequest(
    [Required][StringLength(200, MinimumLength = 2)] string Name,
    [Required][StringLength(1000, MinimumLength = 2)] string Description,
    [Required][StringLength(300, MinimumLength = 2)] string Venue,
    [Required] DateTime StartTime,
    [Required] int TotalCapacity);

public record UpdateEventRequest(
    [Required] Guid Id,
    string? Name,
    string? Description,
    string? Venue,
    DateTime? StartTime,
    int? TotalCapacity);