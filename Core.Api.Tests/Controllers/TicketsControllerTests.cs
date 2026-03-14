using Core.Application.DTO.Requests;
using Core.Application.DTO.Responses;
using Core.Application.Tickets.Services;
using Domain.Shared;
using EventManagement.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Core.Api.Tests.Controllers;

public class TicketsControllerTests
{
    private readonly Mock<ITicketService> _mockService;
    private readonly TicketsController _controller;

    public TicketsControllerTests()
    {
        _mockService = new Mock<ITicketService>();
        _controller = new TicketsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAvailability_ReturnsOk_WhenEventExists()
    {
        var eventId = Guid.NewGuid();
        var availability = new TicketAvailability(new List<PricingTierAvailability>
        {
            new PricingTierAvailability(50, "VIP"),
            new PricingTierAvailability(100, "General")
        });

        _mockService
            .Setup(s => s.GetTicketAvailability(eventId))
            .ReturnsAsync(Result<TicketAvailability>.Success(availability));

        var result = await _controller.GetTicketAvailability(eventId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedValue = Assert.IsType<TicketAvailability>(okResult.Value);
        Assert.Equal(2, returnedValue.TierAvailabilities.Count);
        Assert.Equal("VIP", returnedValue.TierAvailabilities[0].Name);
    }

    [Fact]
    public async Task GetAvailability_ReturnsNotFound_WhenEventDoesNotExist()
    {
        var eventId = Guid.NewGuid();
        var errorMessage = "Test Error";

        _mockService
            .Setup(s => s.GetTicketAvailability(eventId))
            .ReturnsAsync(Result<TicketAvailability>.Fail(errorMessage));

        var result = await _controller.GetTicketAvailability(eventId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(errorMessage, notFoundResult.Value);
    }

    [Fact]
    public async Task PurchaseTickets_ReturnsOk_WhenSuccessful()
    {
        var request = new PurchaseTicketRequest(Guid.NewGuid(),
            Guid.NewGuid(),
            2,
            "test@example.com");

        var response = new PurchasedTicketsResponse(new List<PurchasedTicket>
        {
            new PurchasedTicket("Test Event", "VIP", 200, 2.00m),
            new PurchasedTicket("Test Event", "VIP", 200, 2.00m)
        });

        _mockService
            .Setup(s => s.PurchaseTickets(request))
            .ReturnsAsync(Result<PurchasedTicketsResponse>.Success(response));

        var result = await _controller.PurchaseTickets(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedValue = Assert.IsType<PurchasedTicketsResponse>(okResult.Value);
        Assert.Equal(2, returnedValue.PurchasedTickets.Count);
    }

    [Fact]
    public async Task PurchaseTickets_ReturnsBadRequest_WhenSoldOutOrConcurrencyConflict()
    {
        var request = new PurchaseTicketRequest(Guid.NewGuid(),
            Guid.NewGuid(),
            2,
            "test@example.com");

        var errorMessage = "Test Error";

        _mockService
            .Setup(s => s.PurchaseTickets(request))
            .ReturnsAsync(Result<PurchasedTicketsResponse>.Fail(errorMessage));

        var result = await _controller.PurchaseTickets(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(badRequestResult.Value, errorMessage);
    }
}