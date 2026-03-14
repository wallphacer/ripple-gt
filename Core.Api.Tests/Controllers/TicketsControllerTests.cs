using System;
using Core.Api.Controllers;
using Core.Application.DTO.Requests;
using Core.Application.DTO.Responses;
using Core.Application.DTO.Tickets.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Core.Api.Tests.Controllers;

public class TicketsControllerTests
{
    private readonly Mock<ITicketsService> _mockService;
    private readonly TicketsController _controller;

    public TicketsControllerTests()
    {
        _mockService = new Mock<ITicketsService>();
        _controller = new TicketsController(_mockService.Object);
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
