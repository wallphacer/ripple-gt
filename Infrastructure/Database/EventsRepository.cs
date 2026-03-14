using Core.Application.Events;
using Domain.Events;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class EventsRepository : IEventsRepository
{
    private readonly EventManagementDbContext _context;
    public EventsRepository(EventManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Event>> GetAllEvents()
    {
        // Purely a View of the objects
        // We don't need to track these changes
        return await _context.Events
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        // This is actually going to be used to find things that will need to be changed, so we should track
        return await _context.Events
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task AddAsync(Event newEvent)
    {
        await _context.Events.AddAsync(newEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var foundEvent = await _context.Events.FindAsync(id);
        if (foundEvent == null)
        {
            return false;
        }

        _context.Events.Remove(foundEvent);

        await _context.SaveChangesAsync();
        return true;
    }

    // This function looks ridiculous, but it keeps the interface and API of the repository consistent.
    public async Task<Event?> UpdateAsync(Event eventToUpdate)
    {
        await _context.SaveChangesAsync();
        return eventToUpdate;
    }
}