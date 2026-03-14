using Core.Application.DTO.Requests;
using Core.Application.Events;
using Core.Application.Events.Services;
using Domain.Events;
using Moq;
using Xunit;

namespace Core.Application.Tests.Events;

public class EventServiceTests
{
    private readonly Mock<IEventsRepository> _mockRepo;
    private readonly EventsService _service;

    public EventServiceTests()
    {
        _mockRepo = new Mock<IEventsRepository>();
        _service = new EventsService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllEvents_ReturnsSuccess_WithMappedResponses()
    {
        var events = new List<Event>
        {
            Event.Create("Test Event 1", "Test Description 1", "Test Venue 1", DateTime.UtcNow, 10),
            Event.Create("Test Event 2", "Test Description 2", "Test Venue 2", DateTime.UtcNow, 20)
        };
        _mockRepo.Setup(r => r.GetAllEvents()).ReturnsAsync(events);

        var result = await _service.GetAllEvents();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task AddEvent_ReturnsSuccess_WhenPricingTierDoesNotBreakCapacity()
    {
        var tiers = new List<PricingTierDetails> { new("New Pricing Tier", 1000, 5) };
        var request = new CreateEventRequest("Test Event", "Test Description", "Test Venue", DateTime.UtcNow.AddDays(1), 5, tiers);

        var result = await _service.AddEvent(request);

        Assert.True(result.IsSuccess);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task AddEvent_ReturnsFail_WhenNewPricingTierBreachesCapacity()
    {
        var tiers = new List<PricingTierDetails> { new("Over Capacity", 1000, 10) };
        var request = new CreateEventRequest("Test Event", "Test Description", "Test Venue", DateTime.UtcNow.AddDays(1), 5, tiers);

        var result = await _service.AddEvent(request);

        Assert.False(result.IsSuccess);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEvent_UpdatesOnlyProvidedFields()
    {
        var expectedName = "Updated Name";
        var expectedVenue = "Test Venue";
        var eventId = Guid.NewGuid();
        var existing = Event.Create("Test Name", "Test Description", "Test Venue", DateTime.UtcNow, 100);
        SetId(existing, eventId);
        _mockRepo.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(existing);
        var request = new UpdateEventRequest(eventId, expectedName, null, null, null, null, null);

        var result = await _service.UpdateEvent(eventId, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, existing.Name);
        Assert.Equal(expectedVenue, existing.Venue);
    }

    [Fact]
    public async Task DeleteEvent_ReturnsFail_WhenDeletionFailsInRepo()
    {
        var eventId = Guid.NewGuid();
        _mockRepo.Setup(r => r.DeleteAsync(eventId)).ReturnsAsync(false);

        var result = await _service.DeleteEventById(eventId);

        Assert.False(result.IsSuccess);
    }

    private static void SetId<T>(T obj, Guid id)
    {
        typeof(T)
            .GetProperty("Id")?
            .SetValue(obj, id);
    }
}