using Core.Api.Controllers;
using Core.Application.DTO.Requests;
using Core.Application.DTO.Responses;
using Core.Application.Events.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Core.Api.Tests.Controllers;

public class EventControllerTests
{
    private readonly Mock<IEventsService> _mockService;
    private readonly EventsController _controller;

    public EventControllerTests()
    {
        _mockService = new Mock<IEventsService>();

        _controller = new EventsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAlLEvents_ReturnsOK_WhenEventsExists()
    {
        var events = new List<EventResponse>
        {
            new EventResponse(Guid.NewGuid(), "Test Event 1", "Test Description 1", "Test Venue 1", DateTime.UtcNow.AddDays(1), 100, new List<PricingTierResponse>()),
            new EventResponse(Guid.NewGuid(), "Test Event 2", "Test Description 2", "Test Venue 2", DateTime.UtcNow.AddDays(2), 200, new List<PricingTierResponse>())
        };
        var successfulResult = Result<IList<EventResponse>>.Success(events);

        _mockService
            .Setup(s => s.GetAllEvents())
            .ReturnsAsync(successfulResult);

        var result = await _controller.GetEvents();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEvents = Assert.IsType<List<EventResponse>>(okResult.Value);
        Assert.Equal(2, returnedEvents.Count);
    }

    [Fact]
    public async Task GetAllEvents_ReturnsOKWithEmptyList_WhenEventsDontExist()
    {
        var emptyList = new List<EventResponse>();
        var successfulResult = Result<IList<EventResponse>>.Success(emptyList);

        _mockService
            .Setup(s => s.GetAllEvents())
            .ReturnsAsync(successfulResult);

        var result = await _controller.GetEvents();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEvents = Assert.IsType<List<EventResponse>>(okResult.Value);
        Assert.Empty(returnedEvents);
    }

    [Fact]
    public async Task GetEvent_ReturnsOk_WhenEventExists()
    {
        var eventId = Guid.NewGuid();
        var name = "TestName";
        var desc = "TestDesc";
        var venue = "TestVenue";
        var eventDate = DateTime.UtcNow.AddDays(1);
        var capacity = 10;
        var eventResponse = new EventResponse(eventId, name, desc, venue, eventDate, capacity, new List<PricingTierResponse>());
        var successfulResult = Result<EventResponse>.Success(eventResponse);

        _mockService
            .Setup(s => s.GetEventById(eventId))
            .ReturnsAsync(successfulResult);

        var result = await _controller.GetEvent(eventId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEvent = Assert.IsType<EventResponse>(okResult.Value);
        Assert.Equal(eventId, returnedEvent.Id);
        Assert.Equal(name, returnedEvent.Name);
    }

    [Fact]
    public async Task GetEvent_ReturnsNotFound_WhenEventDoesNotExist()
    {
        var eventId = Guid.NewGuid();
        var errorMessage = "Test Error Message";
        var resultError = Result<EventResponse>.Fail(errorMessage);

        _mockService
            .Setup(s => s.GetEventById(eventId))
            .ReturnsAsync(resultError);

        var result = await _controller.GetEvent(eventId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var returnedMessage = Assert.IsType<string>(notFoundResult.Value);
        Assert.Equal(returnedMessage, errorMessage);
    }

    [Fact]
    public async Task GetEvent_ReturnsBadRequest_WhenEmptyGuidSupplied()
    {
        var eventId = Guid.Empty;

        var result = await _controller.GetEvent(eventId);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateEvent_ReturnsCreatedAtAction_WhenSuccessful()
    {
        var request = new CreateEventRequest(
            "Test Name",
            "Test Description",
            "Test Venue",
            DateTime.UtcNow.AddDays(1),
            100,
            new List<PricingTierDetails>());

        var eventResponse = new EventResponse(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Venue,
            request.StartTime,
            request.TotalCapacity,
            new List<PricingTierResponse>());

        var successfulResult = Result<EventResponse>.Success(eventResponse);

        _mockService
            .Setup(s => s.AddEvent(request))
            .ReturnsAsync(successfulResult);

        var result = await _controller.CreateEvent(request);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedEvent = Assert.IsType<EventResponse>(createdAtActionResult.Value);

        Assert.Equal(nameof(_controller.GetEvent), createdAtActionResult.ActionName);
        Assert.Equal(request.Name, returnedEvent.Name);
    }

    [Fact]
    public async Task CreateEvent_ReturnsBadRequest_WhenServiceFails()
    {
        var request = new CreateEventRequest(
            "Test Name",
            "Test Description",
            "Test Venue",
            DateTime.UtcNow.AddDays(1),
            100,
            new List<PricingTierDetails>());
        var errorMessage = "Test Error Message";
        var failedResult = Result<EventResponse>.Fail(errorMessage);

        _mockService
            .Setup(s => s.AddEvent(request))
            .ReturnsAsync(failedResult);

        var result = await _controller.CreateEvent(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

        Assert.Equal(errorMessage, badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateEvent_ReturnsOk_WhenSuccessful()
    {
        var eventId = Guid.NewGuid();
        var request = new UpdateEventRequest(eventId,
            "Updated Name",
            null,
            null,
            null,
            null,
            null);

        var response = new EventResponse(eventId,
            "Updated Name",
            "Original Desc",
            "Test Venue",
            DateTime.UtcNow.AddDays(1),
            100,
            new List<PricingTierResponse>());

        _mockService
            .Setup(s => s.UpdateEvent(eventId, request))
            .ReturnsAsync(Result<EventResponse>.Success(response));

        var result = await _controller.UpdateEvent(eventId, request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(response, okResult.Value);
    }


    [Fact]
    public async Task DeleteEvent_ReturnsNoContent_WhenSuccessful()
    {
        var eventId = Guid.NewGuid();
        _mockService
            .Setup(s => s.DeleteEventById(eventId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteEvent(eventId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteEvent_ReturnsNotFound_WhenEventDoesNotExist()
    {
        var eventId = Guid.NewGuid();
        var error = "Test Error";

        _mockService
            .Setup(s => s.DeleteEventById(eventId))
            .ReturnsAsync(Result<bool>.Fail(error));

        var result = await _controller.DeleteEvent(eventId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(error, notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteEvent_ReturnsBadRequest_WhenIdIsEmpty()
    {
        var emptyId = Guid.Empty;

        var result = await _controller.DeleteEvent(emptyId);

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.DeleteEventById(It.IsAny<Guid>()), Times.Never);
    }
}